//using Cognex.VisionPro;
//using Cognex.VisionPro.Caliper;
//using Cognex.VisionPro.Exceptions;
//using Cognex.VisionPro.ImageFile;

//using Cognex.VisionPro.ToolBlock;
//using System;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using 视觉检测系统.Properties;
//using 视觉检测系统.UI界面.设置;
//using 视觉检测系统.UI界面.设置.存图设置;
//using 视觉检测系统.存图;

//namespace 视觉检测系统.业务流程.本地测试
//{
//    internal class Check
//    {
//        SettingForm settingForm = new SettingForm();
//      public Form1 mainform;
//        private BlockingCollection<ICogImage> LocalImageQueue = new BlockingCollection<ICogImage>(new ConcurrentQueue<ICogImage>(), 10);
//        public ICogImage getCogImage(CogRecordDisplay cogRecordDisplay)
//        {
//            return cogRecordDisplay.Image;
//        }
//        public Check(Form1 form)
//        {
//            mainform = form;
//        }
//        public Check()
//        {

//        }
//        //单个检测
//        SaveImage saveImage = new SaveImage();
//        public bool checkLocalImg(CogToolBlock toolBlock, ICogImage image)
//        {
//            if (toolBlock == null)
//            {
//                MessageBox.Show("请先加载vpp");
//                return false;
//            }
//            toolBlock.Inputs["Input"].Value = image;

//            toolBlock.Run();
//            if (toolBlock.RunStatus.Result == CogToolResultConstants.Accept)
//            {

//                saveImage.SaveVisionImage(mainform.displayStart, true, saveImgSet.isSaveOkSourceImg, "Source");
//                saveImage.SaveVisionImage(mainform.displayEnd, true, saveImgSet.isSaveOkResultImg, "Result");

//            }
//            else
//            {
//                saveImage.SaveVisionImage(mainform.displayStart, false, saveImgSet.isSaveNGSourceImg, "Source");
//                saveImage.SaveVisionImage(mainform.displayEnd, false, saveImgSet.isSaveNGResultImg, "Result");
//            }
//            return toolBlock.RunStatus.Result == CogToolResultConstants.Accept;
//        }
//        public bool mutileCheckLocalImg(CogRecordDisplay Startdisplay, CogToolBlock toolBlock, 加载文件夹 files, CogRecordDisplay Enddisplay)
//        {
//            if (toolBlock == null)
//            {
//                MessageBox.Show("请先加载vpp");
//                return false;
//            }

//            if (files.ImagePathList.Count == 0)
//            {
//                MessageBox.Show("请先加载文件夹");
//                return false;
//            }

//            files.CurrentIndex = 0;

//            // 生产者线程 —— 只在全部图片加载完后才调用 CompleteAdding()
//            Task.Run(() =>
//            {
//                while (files.CurrentIndex < files.ImagePathList.Count)
//                {
//                    string path = files.ImagePathList[files.CurrentIndex];
//                    try
//                    {
//                        ICogImage image;
//                        using (CogImageFile imgFile = new CogImageFile())
//                        {
//                            imgFile.Open(path, CogImageFileModeConstants.Read);
//                            image = imgFile[0];
//                        }

//                        // 队列容量为10，满了会自动阻塞在此处，等消费者取走后再继续
//                        LocalImageQueue.Add(image);

//                        AddLog.WriteLog($"图片入队成功 [{files.CurrentIndex + 1}/{files.ImagePathList.Count}]");

//                        files.CurrentIndex++;
//                    }
//                    catch (Exception ex)
//                    {
//                        AddLog.WriteLog("读取图片失败: " + ex.Message);
//                        files.CurrentIndex++; // 跳过失败的图片，继续下一张
//                    }
//                }

//                // 全部100张都处理完毕后，才通知消费者结束
//                LocalImageQueue.CompleteAdding();
//            });

//            // 消费者线程
//            Task.Run(() =>
//            {


//                    foreach (ICogImage image in LocalImageQueue.GetConsumingEnumerable())
//                {
//                    try
//                    {
//                        AddLog.WriteLog("开始检测");

//                        if (checkLocalImg(toolBlock, image))
//                        {
//                            AddLog.WriteLog("检测完成");

//                            //ICogImage img = toolBlock.Outputs["OutputImage"].Value as ICogImage;
//                            //    if (img == null)
//                            //    {
//                            //        MessageBox.Show("输出图像为空，请检查设置");
//                            //        return;
//                            //    }//先判断输出是否为空在进行显示

//                            Enddisplay.Invoke(new Action(() =>
//                            {
//                                showResourceImg(Startdisplay, toolBlock);
//                                showResultImg(Enddisplay, toolBlock);

//                            }));
//                            //if (toolBlock.RunStatus.Result == CogToolResultConstants.Accept)
//                            //{

//                            //    saveImage.SaveVisionImage(Startdisplay, true, saveImgSet.isSaveOkSourceImg, "Source");
//                            //    saveImage.SaveVisionImage(Enddisplay, true, saveImgSet.isSaveOkResultImg, "Result");

//                            //}
//                            //else
//                            //{
//                            //    saveImage.SaveVisionImage(Startdisplay, false, saveImgSet.isSaveNGSourceImg, "Source");
//                            //    saveImage.SaveVisionImage(Enddisplay, false, saveImgSet.isSaveNGResultImg, "Result");
//                            //}
//                        }
//                        Thread.Sleep(500);
//                    }
//                    catch (Exception ex)
//                    {
//                        AddLog.WriteLog("检测异常: " + ex.Message);
//                    }
//                }

//                AddLog.WriteLog("全部图片检测完成");
//            });

//            return true;
//        }

//        public void showResultImg(CogRecordDisplay display, CogToolBlock toolBlock)
//        {
//            display.Image = null;

//            display.Record = null;

//            try
//            {
//                ICogImage greyImage = null;    // 灰度图
//                ICogImage colorImage = null;   // 彩色图
//                string greyTerminalName = "";
//                string colorTerminalName = "";

//                foreach (CogToolBlockTerminal t in toolBlock.Outputs)
//                {
//                    if (t.Value == null)
//                        continue;

//                    // 判断灰度图
//                    if (t.Value is CogImage8Grey grey)
//                    {
//                        greyImage = grey;
//                        greyTerminalName = t.Name;
//                    }
//                    // 判断24位彩图
//                    else if (t.Value is CogImage24PlanarColor color24)
//                    {
//                        colorImage = color24;
//                        colorTerminalName = t.Name;
//                    }
//                }


//                //if (colorImage != null)
//                //{
//                //    AddLog.WriteLog($"找到彩色输出图像，终端名称：{colorTerminalName}");

//                //}
//                //else if (greyImage != null)
//                //{
//                //    AddLog.WriteLog($"找到灰度输出图像，终端名称：{greyTerminalName}");

//                //}
//                //else
//                //{
//                //    AddLog.WriteLog("输出终端未找到灰度图/彩色图");
//                //}
//                //if (toolBlock.RunStatus.Result == CogToolResultConstants.Accept)
//                //{
//                //AddLog.WriteLog("toolblock运行成功");
//                //if (greyImage == null && colorImage ==null)
//                //{
//                //    string key = "";
//                //    try
//                //    {
//                //        key = toolBlock.UserData["DisplayImageKey"].ToString();
//                //    }
//                //    catch
//                //    {

//                //        MessageBox.Show("未设置任何输出图像，无法进行显示");
//                //        return;
//                //    }

//                //    ICogRecord record = toolBlock.CreateLastRunRecord();

//                //        ICogRecord imageRecord = FindRecordByKey(record, key);

//                //        display.Record = imageRecord;

//                //        display.Fit();
//                //        AddLog.WriteLog("显示脚本图像成功");

//                //}
//                //else if(greyImage != null)
//                //{
//                //    display.Image = greyImage;
//                //    display.Fit();
//                //}
//                //else if(colorImage!=null){
//                //    display.Image = colorImage;
//                //    display.Fit();
//                //}

//                //}
//                //else
//                //{
//                //    MessageBox.Show("运行失败");
//                //}

//                    string key = "";
//                    try
//                    {
//                        key = toolBlock.UserData["DisplayImageKey"].ToString();

//                    }
//                    catch
//                    {

//                        MessageBox.Show("未设置任何输出图像，无法进行显示");
//                        return;
//                    }

//                if (key!="")
//                {
//                    //AddLog.WriteLog(key);
//                    ICogRecord record = toolBlock.CreateLastRunRecord();

//                    ICogRecord imageRecord = FindRecordByKey(record, key);

//                    display.Record = imageRecord;

//                    display.Fit();
//                    AddLog.WriteLog("显示脚本图像成功");
//                }
//                else if (greyImage != null)
//                {
//                    display.Image = greyImage;
//                    display.Fit();
//                }
//                else if (colorImage != null)
//                {
//                    display.Image = colorImage;
//                    display.Fit();
//                }



//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(
//                    "显示结果图失败:" + ex.Message
//                );
//            }
//        }
//        private ICogRecord FindRecordByKey(ICogRecord record, string key)
//        {
//            //先判断当前节点
//            if (record.RecordKey == key)
//            {
//                return record;
//            }


//            //递归查找子节点
//            foreach (ICogRecord subRecord in record.SubRecords)
//            {
//                ICogRecord result = FindRecordByKey(subRecord, key);

//                if (result != null)
//                {
//                    return result;
//                }
//            }


//            return null;
//        }



//        public void showResourceImg(CogRecordDisplay display, CogToolBlock toolBlock)
//        {
//            try
//            {
//                ICogImage resourceImg = toolBlock.Inputs["Input"].Value as ICogImage;

//                if (resourceImg != null)
//                {
//                    display.Image = resourceImg;
//                    display.Fit();
//                }

//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(
//                    "显示原图失败:" + ex.Message
//                );
//            }
//        }
//    }
//}
#region MyRegion
using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.ImageFile;

using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using 视觉检测系统.Properties;
using 视觉检测系统.UI界面.设置;
using 视觉检测系统.UI界面.设置.存图设置;
using 视觉检测系统.存图;

namespace 视觉检测系统.业务流程.本地测试
{
    internal class Check
    {
        SettingForm settingForm = new SettingForm();
        public Form1 mainform;

        // 图片队列（生产者→消费者），容量10
        private BlockingCollection<ICogImage> LocalImageQueue = new BlockingCollection<ICogImage>(new ConcurrentQueue<ICogImage>(), 10);

        // 存图队列（消费者→存图线程），容量20
        private BlockingCollection<SaveItem> SaveQueue = new BlockingCollection<SaveItem>(new ConcurrentQueue<SaveItem>(), 20);

        private readonly object _indexLock = new object();
        private CancellationTokenSource _batchCts;

        private Thread _producerThread;
        private Thread _consumerThread;
        private Thread _saveThread;

        // 存图数据项：只封装原图（结果图在UI线程存）
        private class SaveItem : IDisposable
        {
            public ICogImage SourceImage;
            public bool IsOk;

            public void Dispose()
            {
                (SourceImage as IDisposable)?.Dispose();
            }
        }

        private void ClearQueue()
        {
            while (LocalImageQueue.TryTake(out var img))
            {
                (img as IDisposable)?.Dispose();
            }
        }

        private void ClearSaveQueue()
        {
            while (SaveQueue.TryTake(out var item))
            {
                item?.Dispose();
            }
        }

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

        SaveImage saveImage = new SaveImage();

        // ========== 单个检测 ==========
        public bool checkLocalImg(CogToolBlock toolBlock, ICogImage image)
        {
            if (toolBlock == null)
            {
                MessageBox.Show("请先加载vpp");
                return false;
            }
            if (image == null)
            {
                MessageBox.Show("输入图像为空");
                return false;
            }

        
            foreach (CogToolBlockTerminal t in toolBlock.Inputs)
            {
                t.Value = null;
            }
            foreach (CogToolBlockTerminal t in toolBlock.Outputs)
            {
                t.Value = null;
            }

           
            toolBlock.Inputs["Input"].Value = image;
            toolBlock.Run();
            bool isOk = toolBlock.RunStatus.Result == CogToolResultConstants.Accept;

           
            mainform.displayStart.Image = image;
            mainform.displayStart.Fit();

            showResultImg(mainform.displayEnd, toolBlock);

            if (isOk)
            {
                saveImage.SaveVisionImage(image, true, saveImgSet.isSaveOkSourceImg, "Source");
                saveImage.SaveVisionImage(mainform.displayEnd, true, saveImgSet.isSaveOkResultImg, "Result");
            }
            else
            {
                saveImage.SaveVisionImage(image, false, saveImgSet.isSaveNGSourceImg, "Source");
                saveImage.SaveVisionImage(mainform.displayEnd, false, saveImgSet.isSaveNGResultImg, "Result");
            }

            return isOk;
        }

        // ========== 批量检测 ==========
        public bool mutileCheckLocalImg(CogRecordDisplay Startdisplay, CogToolBlock toolBlock, 加载文件夹 files, CogRecordDisplay Enddisplay)
        {
            if (toolBlock == null)
            {
                MessageBox.Show("请先加载vpp");
                return false;
            }
            if (files.ImagePathList == null || files.ImagePathList.Count == 0)
            {
                MessageBox.Show("请先加载文件夹");
                return false;
            }

            // 停止上一轮
            _batchCts?.Cancel();
            _batchCts?.Dispose();
            ClearQueue();
            ClearSaveQueue();
            _batchCts = new CancellationTokenSource();
            var token = _batchCts.Token;

            lock (_indexLock)
            {
                files.CurrentIndex = 0;
            }

            // ============================================================
            // 存图线程（STA）：只存原图，不需要Display控件
            // ============================================================
            _saveThread = new Thread(() =>
            {
                try
                {
                    foreach (var item in SaveQueue.GetConsumingEnumerable())
                    {
                        try
                        {
                            saveImage.SaveVisionImage(
                                item.SourceImage, item.IsOk,
                                item.IsOk ? saveImgSet.isSaveOkSourceImg : saveImgSet.isSaveNGSourceImg,
                                "Source");
                        }
                        catch (Exception ex)
                        {
                            AddLog.WriteLog("存图异常: " + ex.Message);
                        }
                        finally
                        {
                            item.Dispose();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    AddLog.WriteLog("存图线程终止");
                }
                catch (Exception ex)
                {
                    AddLog.WriteLog("存图线程异常：" + ex.Message);
                }
                finally
                {
                  //  AddLog.WriteLog("存图线程结束");
                }
            });
            _saveThread.SetApartmentState(ApartmentState.STA);
            _saveThread.IsBackground = true;
            _saveThread.Start();

            // ============================================================
            // 生产者线程（STA）：读图入队
            // ============================================================
            _producerThread = new Thread(() =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        int curIdx;
                        lock (_indexLock)
                        {
                            curIdx = files.CurrentIndex;
                            if (curIdx >= files.ImagePathList.Count) break;
                            files.CurrentIndex++;
                        }

                        string path = files.ImagePathList[curIdx];
                        ICogImage image = null;
                        try
                        {
                            using (CogImageFile imgFile = new CogImageFile())
                            {
                                imgFile.Open(path, CogImageFileModeConstants.Read);
                                image = imgFile[0];
                            }
                            LocalImageQueue.Add(image, token);
                            AddLog.WriteLog($"图片入队成功 [{curIdx + 1}/{files.ImagePathList.Count}]");
                        }
                        catch (Exception ex)
                        {
                            AddLog.WriteLog("读取图片失败: " + ex.Message);
                            (image as IDisposable)?.Dispose();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    AddLog.WriteLog("图片读取任务终止");
                }
                catch (Exception ex)
                {
                    AddLog.WriteLog("生产者异常：" + ex.Message);
                }
                finally
                {
                    LocalImageQueue.CompleteAdding();
                    AddLog.WriteLog("全部图片入队完成");
                }
            });
            _producerThread.SetApartmentState(ApartmentState.STA);
            _producerThread.IsBackground = true;
            _producerThread.Start();

      
            _consumerThread = new Thread(() =>
            {
                try
                {
                    foreach (ICogImage image in LocalImageQueue.GetConsumingEnumerable(token))
                    {
                        if (token.IsCancellationRequested) break;
                        if (image == null) continue;

                        ICogImage imageForSave = image;
                        try
                        {
                            AddLog.WriteLog("开始检测");
                            toolBlock.Inputs["Input"].Value = image;
                            toolBlock.Run();
                            AddLog.WriteLog("检测完成");

                            bool isOk = toolBlock.RunStatus.Result == CogToolResultConstants.Accept;

                            // Invoke到UI线程：刷新显示 + 存结果图
                            Enddisplay.Invoke(new Action(() =>
                            {
                                try
                                {
                                    // 清空画布
                                    Startdisplay.Image = null;
                                    Startdisplay.Record = null;
                                    Startdisplay.StaticGraphics.Clear();
                                    Startdisplay.InteractiveGraphics.Clear();
                                    Enddisplay.Image = null;
                                    Enddisplay.Record = null;
                                    Enddisplay.StaticGraphics.Clear();
                                    Enddisplay.InteractiveGraphics.Clear();

                                    // ★修复：显示原图直接用image变量，不从toolBlock.Inputs取
                                    //   toolBlock.Run()后Inputs["Input"].Value可能已被释放
                                    if (image != null)
                                    {
                                        Startdisplay.Image = image;
                                        Startdisplay.Fit();
                                    }

                                    // 显示结果图（用原始toolBlock，UserData完整）
                                    showResultImg(Enddisplay, toolBlock);

                                    // 存结果图（displayEnd已初始化，无ActiveX问题）
                                    saveImage.SaveVisionImage(
                                        Enddisplay, isOk,
                                        isOk ? saveImgSet.isSaveOkResultImg : saveImgSet.isSaveNGResultImg,
                                        "Result");
                                }
                                catch (Exception ex)
                                {
                                    AddLog.WriteLog("UI刷新/结果图存图异常: " + ex.Message);
                                }
                            }));

                            // 原图放入存图队列
                            SaveQueue.Add(new SaveItem
                            {
                                SourceImage = imageForSave,
                                IsOk = isOk
                            }, token);
                        }
                        catch (Exception ex)
                        {
                            AddLog.WriteLog("检测异常: " + ex.Message);
                            (imageForSave as IDisposable)?.Dispose();
                        }
                        Thread.Sleep(500);
                    }
                    AddLog.WriteLog("全部图片检测完成");
                }
                catch (OperationCanceledException)
                {
                    AddLog.WriteLog("批量检测手动停止");
                }
                catch (Exception ex)
                {
                    AddLog.WriteLog("消费者线程异常：" + ex.Message);
                }
                finally
                {
                    SaveQueue.CompleteAdding();
                }
            });
            _consumerThread.SetApartmentState(ApartmentState.STA);
            _consumerThread.IsBackground = true;
            _consumerThread.Start();

            return true;
        }

        // 停止批量检测
        public void StopMultiCheck()
        {
            _batchCts?.Cancel();
            ClearQueue();
            ClearSaveQueue();
            AddLog.WriteLog("已停止批量检测");
        }

        //public void showResultImg(CogRecordDisplay display, CogToolBlock toolBlock)
        //{
        //    display.Image = null;
        //    display.Record = null;

        //    try
        //    {
        //        ICogImage greyImage = null;
        //        ICogImage colorImage = null;
        //        string greyTerminalName = "";
        //        string colorTerminalName = "";

        //        foreach (CogToolBlockTerminal t in toolBlock.Outputs)
        //        {
        //            if (t.Value == null)
        //                continue;

        //            if (t.Value is CogImage8Grey grey)
        //            {
        //                greyImage = grey;
        //                greyTerminalName = t.Name;
        //            }
        //            else if (t.Value is CogImage24PlanarColor color24)
        //            {
        //                colorImage = color24;
        //                colorTerminalName = t.Name;
        //            }
        //        }

        //        string key = "";
        //        if (toolBlock.UserData.Contains("DisplayImageKey"))
        //        {
        //            key = toolBlock.UserData["DisplayImageKey"]?.ToString() ?? "";
        //        }
        //        //else
        //        //{
        //        //    AddLog.WriteLog("未设置任何输出图像，无法进行显示");
        //        //    return;
        //        //}

        //        if (key != "")
        //        {
        //            ICogRecord record = toolBlock.CreateLastRunRecord();
        //            ICogRecord imageRecord = FindRecordByKey(record, key);
        //            if (imageRecord != null)
        //            {
        //                display.Record = imageRecord;
        //                display.Fit();
        //                AddLog.WriteLog("显示脚本图像成功");
        //            }
        //            else if (greyImage != null)
        //            {
        //                display.Image = greyImage;
        //                display.Fit();
        //            }
        //            else if (colorImage != null)
        //            {
        //                display.Image = colorImage;
        //                display.Fit();
        //            }
        //        }
        //        else if (greyImage != null)
        //        {
        //            display.Image = greyImage;
        //            display.Fit();
        //        }
        //        else if (colorImage != null)
        //        {
        //            display.Image = colorImage;
        //            display.Fit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddLog.WriteLog("显示结果图失败:" + ex.Message);
        //    }
        //}
        public void showResultImg(CogRecordDisplay display, CogToolBlock toolBlock)
        {
            if (display == null || toolBlock == null)
                return;

            try
            {
                // =====================================================
                // 1. 清空显示
                // =====================================================
                display.Record = null;
                display.Image = null;
                display.StaticGraphics.Clear();
                display.InteractiveGraphics.Clear();


                // =====================================================
                // 2. 获取 ToolBlock 输出的图片
                //    优先找 OutputImage
                //    如果没有，就自动找第一个 ICogImage 输出
                // =====================================================
                ICogImage image = null;

                if (toolBlock.Outputs.Contains("OutputImage"))
                {
                    image = toolBlock.Outputs["OutputImage"].Value as ICogImage;
                }

                // 如果 OutputImage 没有图片，就自动寻找其他输出图片
                if (image == null)
                {
                    foreach (CogToolBlockTerminal output in toolBlock.Outputs)
                    {
                        if (output.Value is ICogImage)
                        {
                            image = output.Value as ICogImage;

                            //AddLog.WriteLog(
                            //    "自动找到输出图像：" + output.Name
                            //);

                            break;
                        }
                    }
                }

                if (image == null)
                {
                    AddLog.WriteLog("没有找到任何 ICogImage 输出");
                    return;
                }


                // =====================================================
                // 3. 只把“当前输出图片”作为底图
                // =====================================================
                display.Image = image;


                // =====================================================
                // 4. 获取运行 Record
                //    注意：
                //    这里绝对不能：
                //
                //    display.Record = lastRecord;
                //
                //    否则 Record 中自己的图片会覆盖当前输出图片
                // =====================================================
                ICogRecord lastRecord = toolBlock.CreateLastRunRecord();

                if (lastRecord == null)
                {
                    AddLog.WriteLog("LastRunRecord为空");
                    display.Fit();
                    return;
                }


                // =====================================================
                // 5. 从 Record 中递归寻找 Graphic
                //    只拿 Graphic，不拿 Record 里的 Image
                // =====================================================
                Action<ICogRecord> findGraphic = null;

                findGraphic = record =>
                {
                    if (record == null)
                        return;

                    try
                    {
                        // ---------------------------------------------
                        // 当前 Record 是 Graphic
                        // ---------------------------------------------
                        ICogGraphic graphic =
                            record.Content as ICogGraphic;

                        if (graphic != null)
                        {
                            try
                            {
                                display.StaticGraphics.Add(
                                    graphic,
                                    "ToolBlockGraphics"
                                );
                            }
                            catch
                            {
                                // 忽略重复Graphic
                            }
                        }


                        // ---------------------------------------------
                        // 当前 Record 是 Graphic 集合
                        // ---------------------------------------------
                        CogGraphicCollection graphicCollection =
                            record.Content as CogGraphicCollection;

                        if (graphicCollection != null)
                        {
                            foreach (ICogGraphic item in graphicCollection)
                            {
                                if (item == null)
                                    continue;

                                try
                                {
                                    display.StaticGraphics.Add(
                                        item,
                                        "ToolBlockGraphics"
                                    );
                                }
                                catch
                                {
                                    // 忽略重复Graphic
                                }
                            }
                        }


                        // ---------------------------------------------
                        // 继续寻找子Record
                        // ---------------------------------------------
                        foreach (ICogRecord subRecord in record.SubRecords)
                        {
                            findGraphic(subRecord);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLog.WriteLog(
                            "读取Graphic失败：" + ex.Message
                        );
                    }
                };


                // 开始寻找Graphic
                findGraphic(lastRecord);


                // =====================================================
                // 6. 最终显示
                // =====================================================
                display.Fit();

               // AddLog.WriteLog("检测结果显示完成：当前输出图片 + 程序Graphic");
            }
            catch (Exception ex)
            {
                AddLog.WriteLog(
                    "显示检测结果失败：" + ex.Message
                );
            }
        }
        private ICogRecord FindRecordByKey(ICogRecord record, string key)
        {
            if (record == null) return null;
            if (record.RecordKey == key)
            {
                return record;
            }
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
                AddLog.WriteLog("显示原图失败:" + ex.Message);
            }
        }
    }
}

#endregion


