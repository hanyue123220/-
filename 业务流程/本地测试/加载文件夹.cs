using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using 视觉检测系统.通用函数层;

public class 加载文件夹
{
    // 所有图片路径集合
    public List<string> ImagePathList { get; set; }
    // 当前索引
    public int CurrentIndex { get; set; }
    // 支持图片格式
    private readonly string[] imgFormat = { ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };

    public 加载文件夹()
    {
        ImagePathList = new List<string>();
        CurrentIndex = 0;
    }

    /// <summary>
    /// 选择文件夹 读取里面所有图片
    /// </summary>
    /// <returns>是否读取成功</returns>
    public bool ChooseFolder()
    {
        ImagePathList.Clear();
        CurrentIndex = 0;

        FolderBrowserDialog dialog = new FolderBrowserDialog();
        dialog.Description = "请选择图片文件夹";
        if (dialog.ShowDialog() != DialogResult.OK)
            return false;

        string folderPath = dialog.SelectedPath;
        // 获取文件夹所有文件
        string[] files = Directory.GetFiles(folderPath);
        foreach (var file in files)
        {
            string ext = Path.GetExtension(file).ToLower();
            if (Array.Exists(imgFormat, p => p == ext))
            {
                ImagePathList.Add(file);
            }
        }
        return ImagePathList.Count > 0;
    }

    /// <summary>
    /// 加载当前图片显示到CogRecordDisplay
    /// </summary>
    /// <param name="cogDisplay">显示控件</param>
    public void ShowCurrentImage(CogRecordDisplay cogDisplay)
    {
        if (ImagePathList == null || ImagePathList.Count == 0) return;
        if (CurrentIndex < 0 || CurrentIndex >= ImagePathList.Count) return;

        try
        {
            string path = ImagePathList[CurrentIndex];
            using (CogImageFile imgFile = new CogImageFile())
            {
                // 先创建对象，再Open文件
                imgFile.Open(path, CogImageFileModeConstants.Read);
                ICogImage img = imgFile[0];
                UpdateUi.Update(cogDisplay, () =>
                {
                    cogDisplay.Image = img;
                    cogDisplay.Fit();
                });
            
            }
        }
        catch
        {
            MessageBox.Show("图片加载失败");
        }
    }

    /// <summary>
    /// 上一张
    /// </summary>
    public void PrevImage()
    {
        if (CurrentIndex > 0)
            CurrentIndex--;
    }

    /// <summary>
    /// 下一张
    /// </summary>
    public void NextImage()
    {
        if (CurrentIndex < ImagePathList.Count - 1)
            CurrentIndex++;
    }
}