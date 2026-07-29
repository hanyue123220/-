using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.ImageFile;

using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 视觉检测系统.Properties;

namespace 视觉检测系统.业务流程.本地测试
{
    internal class Check
    {
        Form1 mainform;
        private BlockingCollection<ICogImage> LocalImageQueue = new BlockingCollection<ICogImage>(new ConcurrentQueue<ICogImage>(), 10);
        public ICogImage getCogImage(CogRecordDisplay cogRecordDisplay)
        {
            return cogRecordDisplay.Image;
        }
        public Check(Form1 form)
        {
            mainform = form;
        }
        public Check()
        {

        }
        //单个检测
        public bool checkLocalImg(CogToolBlock toolBlock, ICogImage image)
        {
            if (toolBlock == null)
            {
                MessageBox.Show("请先加载vpp");
                return false;
            }
            toolBlock.Inputs["Input"].Value = image;
        
            toolBlock.Run();
            return toolBlock.RunStatus.Result == CogToolResultConstants.Accept;
        }
        public bool mutileCheckLocalImg(CogRecordDisplay Startdisplay, CogToolBlock toolBlock, 加载文件夹 files, CogRecordDisplay Enddisplay)
        {
            if (toolBlock == null)
            {
                MessageBox.Show("请先加载vpp");
                return false;
            }

            if (files.ImagePathList.Count == 0)
            {
                MessageBox.Show("请先加载文件夹");
                return false;
            }

            files.CurrentIndex = 0;

            // 生产者线程 —— 只在全部图片加载完后才调用 CompleteAdding()
            Task.Run(() =>
            {
                while (files.CurrentIndex < files.ImagePathList.Count)
                {
                    string path = files.ImagePathList[files.CurrentIndex];
                    try
                    {
                        ICogImage image;
                        using (CogImageFile imgFile = new CogImageFile())
                        {
                            imgFile.Open(path, CogImageFileModeConstants.Read);
                            image = imgFile[0];
                        }

                        // 队列容量为10，满了会自动阻塞在此处，等消费者取走后再继续
                        LocalImageQueue.Add(image);

                        AddLog.WriteLog($"图片入队成功 [{files.CurrentIndex + 1}/{files.ImagePathList.Count}]");

                        files.CurrentIndex++;
                    }
                    catch (Exception ex)
                    {
                        AddLog.WriteLog("读取图片失败: " + ex.Message);
                        files.CurrentIndex++; // 跳过失败的图片，继续下一张
                    }
                }

                // 全部100张都处理完毕后，才通知消费者结束
                LocalImageQueue.CompleteAdding();
            });

            // 消费者线程
            Task.Run(() =>
            {
           

                    foreach (ICogImage image in LocalImageQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        AddLog.WriteLog("开始检测");
                       
                        if (checkLocalImg(toolBlock, image))
                        {
                            AddLog.WriteLog("检测完成");

                        //ICogImage img = toolBlock.Outputs["OutputImage"].Value as ICogImage;
                        //    if (img == null)
                        //    {
                        //        MessageBox.Show("输出图像为空，请检查设置");
                        //        return;
                        //    }//先判断输出是否为空在进行显示

                            Enddisplay.Invoke(new Action(() =>
                            {
                                showResourceImg(Startdisplay, toolBlock);
                                showResultImg(Enddisplay, toolBlock);
                            }));
                        }
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        AddLog.WriteLog("检测异常: " + ex.Message);
                    }
                }

                AddLog.WriteLog("全部图片检测完成");
            });

            return true;
        }

        public void showResultImg(CogRecordDisplay display, CogToolBlock toolBlock)
        {
            display.Image = null;
        
            display.Record = null;

            try
            {
                ICogImage greyImage = null;    // 灰度图
                ICogImage colorImage = null;   // 彩色图
                string greyTerminalName = "";
                string colorTerminalName = "";

                foreach (CogToolBlockTerminal t in toolBlock.Outputs)
                {
                    if (t.Value == null)
                        continue;

                    // 判断灰度图
                    if (t.Value is CogImage8Grey grey)
                    {
                        greyImage = grey;
                        greyTerminalName = t.Name;
                    }
                    // 判断24位彩图
                    else if (t.Value is CogImage24PlanarColor color24)
                    {
                        colorImage = color24;
                        colorTerminalName = t.Name;
                    }
                }

             
                if (colorImage != null)
                {
                    AddLog.WriteLog($"找到彩色输出图像，终端名称：{colorTerminalName}");
               
                }
                else if (greyImage != null)
                {
                    AddLog.WriteLog($"找到灰度输出图像，终端名称：{greyTerminalName}");
                 
                }
                else
                {
                    AddLog.WriteLog("输出终端未找到灰度图/彩色图");
                }
                if (toolBlock.RunStatus.Result == CogToolResultConstants.Accept)
                {
                    AddLog.WriteLog("toolblock运行成功");
                    if (greyImage == null && colorImage ==null)
                    {
                    
                        string key = toolBlock.UserData["DisplayImageKey"].ToString();

                
                        ICogRecord record = toolBlock.CreateLastRunRecord();

                        ICogRecord imageRecord = FindRecordByKey(record, key);

                        display.Record = imageRecord;

                        display.Fit();
                        AddLog.WriteLog("显示脚本图像成功");
                    }
                    else if(greyImage != null)
                    {
                        display.Image = greyImage;
                    }
                    else if(colorImage!=null){
                        display.Image = colorImage;
                    }
             
                }
                else
                {
                    MessageBox.Show("运行失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "显示结果图失败:" + ex.Message
                );
            }
        }
        private ICogRecord FindRecordByKey(ICogRecord record, string key)
        {
            //先判断当前节点
            if (record.RecordKey == key)
            {
                return record;
            }


            //递归查找子节点
            foreach (ICogRecord subRecord in record.SubRecords)
            {
                ICogRecord result = FindRecordByKey(subRecord, key);

                if (result != null)
                {
                    return result;
                }
            }


            return null;
        }



        public void showResourceImg(CogRecordDisplay display, CogToolBlock toolBlock)
        {
            try
            {
                ICogImage resourceImg = toolBlock.Inputs["Input"].Value as ICogImage;

                if (resourceImg != null)
                {
                    display.Image = resourceImg;
                    display.Fit();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "显示原图失败:" + ex.Message
                );
            }
        }
    }
}
