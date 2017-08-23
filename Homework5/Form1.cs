using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace Homework5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Code starts
        Bitmap a, b, c, d;
        Rectangle clone;


        //讀圖按鈕
        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog tomato = new OpenFileDialog();
            if(tomato.ShowDialog()==DialogResult.OK)
            {
                a = new Bitmap(tomato.FileName);
                pictureBox1.Image = a;
                clone = new Rectangle(0, 0, a.Width, a.Height);
                
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {//色彩的
            if(a==null)
            {
                
                MessageBox.Show("請先載入圖片");
                return;
            }
            int temp;
            int[,,] source = GetRGBData(a);
            int[,,] newimg = new int[a.Width, a.Height, 3];
            double temp1, temp2, min;
            double[,,] hsi = new double[a.Width, a.Height, 3];
            double R, G, B, xx, yy, zz, rr = 0, bb = 0, gg = 0, flag = 0;
            int[,] hist1 = new int[2, 256];
            int[] hist2 = new int[256];
            int count;
            if (comboBox1.SelectedIndex == 0)
            {

            }
            else if (comboBox1.SelectedIndex == 1)
            {
                for (int i = 0; i < a.Width; i++)//轉HSV
                {
                    for (int j = 0; j < a.Height; j++)
                    {
                        //source[i, j, 0] = 0;
                        //source[i, j, 1] = 0;
                        //source[i, j, 2] = 0;
                        R = Convert.ToDouble(source[i, j, 0]) / 255;
                        G = Convert.ToDouble(source[i, j, 1]) / 255;
                        B = Convert.ToDouble(source[i, j, 2]) / 255;
                        if (R + G + B == 0)
                        {
                            R = 0.0001;
                            G = 0.0001;
                            B = 0.0001;
                        }
                        rr = R / (R + G + B);
                        gg = G / (R + G + B);
                        bb = B / (R + G + B);
                        temp1 = 0.5 * (2 * R - G - B);
                        temp2 = Math.Sqrt((R - G) * (R - G) + (R - B) * (G - B));
                        hsi[i, j, 0] = Math.Acos(temp1 / temp2) * 180 / Math.PI;
                        if (!double.IsNaN(hsi[i, j, 0]))
                            hsi[i, j, 0] = (B > G) ? 360 - hsi[i, j, 0] : hsi[i, j, 0];
                        else
                            hsi[i, j, 0] = 0;
                        min = 255;
                        if (R < min)
                            min = R;
                        if (G < min)
                            min = G;
                        if (B < min)
                            min = B;
                        hsi[i, j, 1] = 1.0 - 3.0 * min / (R + G + B);
                        hsi[i, j, 2] = (R + G + B) / 3;
                        //int a = 1000;
                    }
                }
                for (int i = 0; i < a.Width; i++)
                {
                    for (int j = 0; j < a.Height; j++)
                    {
                        hsi[i, j, 2] *= 1.6;
                    }
                }//cdf
               

                for (int i = 0; i < a.Width; i++) //轉回
                {
                    for (int j = 0; j < a.Height; j++)
                    {
                        if (hsi[i, j, 0] >= 0 && hsi[i, j, 0] <= 120)
                            flag = 0;
                        else if (hsi[i, j, 0] > 120 && hsi[i, j, 0] <= 240)
                        {
                            hsi[i, j, 0] -= 120;
                            flag = 1;
                        }
                        else if (hsi[i, j, 0] > 240 && hsi[i, j, 0] <= 360)
                        {
                            hsi[i, j, 0] -= 240;
                            flag = 2;
                        }

                        xx = (1 - hsi[i, j, 1]) / 3;
                        temp1 = hsi[i, j, 1] * Math.Cos(hsi[i, j, 0] * Math.PI / 180.0);
                        temp2 = Math.Cos((60 - hsi[i, j, 0]) * Math.PI / 180.0);
                        yy = (1 + temp1 / temp2) / 3;
                        zz = 1 - (xx + yy);

                        if (flag == 0)
                        {
                            bb = xx;
                            rr = yy;
                            gg = zz;
                        }
                        else if (flag == 1)
                        {
                            rr = xx;
                            gg = yy;
                            bb = zz;
                        }
                        else if (flag == 2)
                        {
                            gg = xx;
                            bb = yy;
                            rr = zz;
                        }
                        rr = rr * hsi[i, j, 2] * 3 * 255;
                        if (rr > 255)
                            rr = 255;
                        gg = gg * hsi[i, j, 2] * 3 * 255;
                        if (gg > 255)
                            gg = 255;
                        bb = bb * hsi[i, j, 2] * 3 * 255;
                        if (bb > 255)
                            bb = 255;
                        source[i, j, 0] = Convert.ToInt16(rr);
                        source[i, j, 1] = Convert.ToInt16(gg);
                        source[i, j, 2] = Convert.ToInt16(bb);

                    }
                }
                
            }

            else
            {
                for (int i = 0; i < a.Width; i++)
                    for (int j = 0; j < a.Height; j++)
                        for (int k = 0; k < 3; k++)
                        {
                            temp = (int)(source[i, j, k] * 0.2);
                            if (temp > 255)
                                temp = 255;
                            source[i, j, k] = temp;
                        }
            }
            b=setRGBData(source);
            pictureBox3.Image = b;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("請先完成轉換");
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "所有檔案|*.*|BMP File|*.bmp|JPEG File|*.jpg|GIF File|*.gif|PNG File|*.png|TIFF File|*.tiff";
                saveFileDialog1.FilterIndex = 3;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName != "")
                {
                    Bitmap processedBitmap = c;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case 2:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case 3:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case 4:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 5:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                            break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片、並選擇色彩變化");
                return;
            }
            if (b == null)
            {
                MessageBox.Show("請先選擇色彩變化");
                return;
            }
            int[,,] source = GetRGBData(b);
            
            double temp1, temp2,min;
            double[,,] hsi = new double[b.Width, b.Height, 3];
            int[,,] newimg = new int[b.Width, b.Height, 3];
            double R, G, B,xx,yy,zz,rr=0,bb=0,gg=0,flag=0;
            int[,] hist1 = new int[2,256];
            int[] hist2 = new int[256];
            int count;
            for (int i = 0; i < b.Width; i++)//轉HSV
            {
                for (int j = 0; j < b.Height; j++)
                {
                    //source[i, j, 0] = 0;
                    //source[i, j, 1] = 0;
                    //source[i, j, 2] = 0;
                    R = Convert.ToDouble(source[i, j, 0]) / 255;
                    G = Convert.ToDouble(source[i, j, 1]) / 255;
                    B = Convert.ToDouble(source[i, j, 2]) / 255;
                    if (R + G + B == 0)
                    {
                        R = 0.0001;
                        G = 0.0001;
                        B = 0.0001;
                    }
                    rr = R / (R + G + B);
                    gg = G / (R + G + B);
                    bb = B / (R + G + B);
                    temp1 = 0.5 * (2 * R - G - B);
                    temp2 = Math.Sqrt((R - G)*(R - G) + (R - B) * (G - B));
                    hsi[i, j, 0] = Math.Acos(temp1 / temp2)*180/Math.PI;
                    if (!double.IsNaN(hsi[i, j, 0]))
                        hsi[i, j, 0] = (B > G) ? 360 - hsi[i, j, 0] : hsi[i, j, 0];
                    else
                        hsi[i, j, 0] = 0;
                    min = 255;
                    if (R < min)
                        min = R;
                    if (G < min)
                        min = G;
                    if (B < min)
                        min = B;
                    hsi[i, j, 1] = 1.0 - 3.0 *min/ (R + G + B);
                    hsi[i, j, 2] = (R + G + B) / 3;
                    //int a = 1000;
                }
            }
            //做直方圖
            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    hsi[i, j, 2] *= 255;
                    hist1[0, (int)hsi[i, j, 2]]++;
                }
            }//cdf
            count = 0;
            for(int i=0;i<256;i++)
            {
                if (hist1[0, i] != 0)
                {
                    count = hist1[0, i] + count;
                    hist1[1, i] = count;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (hist1[0, i] != 0)
                {
                    hist2[i] = hist1[1, i] * 255 / count;
                }
            }

            for(int i=0;i<b.Width;i++)
            {
                for(int j=0;j<b.Height;j++)
                {
                    hsi[i, j, 2] = hist2[(int)hsi[i, j, 2]];
                    source[i, j, 0] = (int)hsi[i, j, 2];
                    source[i, j, 1] = (int)hsi[i, j, 2];
                    source[i, j, 2] = (int)hsi[i, j, 2];
                    hsi[i, j, 2] /= 255;
                }
            }

            for (int i=0;i<b.Width;i++) //轉回
            {
                for(int j =0;j<b.Height;j++)
                {
                    if (hsi[i, j, 0] >= 0 && hsi[i, j, 0] <= 120)
                        flag = 0;
                    else if (hsi[i, j, 0] > 120 && hsi[i, j, 0] <= 240)
                    {
                        hsi[i, j, 0] -= 120;
                        flag = 1;
                    }
                    else if (hsi[i, j, 0] > 240 && hsi[i, j, 0] <= 360)
                    {
                        hsi[i, j, 0] -= 240;
                        flag = 2;
                    }

                    xx = (1 - hsi[i, j, 1]) / 3;
                    temp1 = hsi[i, j, 1] * Math.Cos(hsi[i, j, 0] * Math.PI / 180.0);
                    temp2 = Math.Cos((60 - hsi[i, j, 0]) * Math.PI / 180.0);
                    yy = (1 + temp1 / temp2) / 3;
                    zz = 1 - (xx + yy);

                    if (flag == 0)
                    {
                        bb = xx;
                        rr = yy;
                        gg = zz;
                    }
                    else if (flag == 1)
                    {
                        rr = xx;
                        gg = yy;
                        bb = zz;
                    }
                    else if (flag == 2)
                    {
                        gg = xx;
                        bb = yy;
                        rr = zz;
                    }
                    rr = rr * hsi[i, j, 2] * 3 * 255;
                    if (rr > 255)
                        rr = 255;
                    gg = gg * hsi[i, j, 2] * 3 * 255;
                    if (gg > 255)
                        gg = 255;
                    bb = bb * hsi[i, j, 2] * 3 * 255;
                    if (bb > 255)
                        bb = 255;
                    newimg[i, j, 0] = Convert.ToInt16(rr);
                    newimg[i, j, 1] = Convert.ToInt16(gg);
                    newimg[i, j, 2] = Convert.ToInt16(bb);

                }
            }
            c = setRGBData(newimg);
            pictureBox2.Image = c;
        }
        public static int[,,] GetRGBData(Bitmap bitImg)
        {
            int height = bitImg.Height;
            int width = bitImg.Width;
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // get the starting memory place
            IntPtr imgPtr = bitmapData.Scan0;
            //scan width
            int stride = bitmapData.Stride;
            //scan ectual
            int widthByte = width * 3;
            // the byte num of padding
            int skipByte = stride - widthByte;
            //set the place to save values
            int[,,] rgbData = new int[width, height, 3];
            #region
            unsafe//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        //B channel
                        rgbData[i, j, 2] = p[0];
                        p++;
                        //g channel
                        rgbData[i, j, 1] = p[0];
                        p++;
                        //R channel
                        rgbData[i, j, 0] = p[0];
                        p++;
                    }
                    p += skipByte;
                }
            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return rgbData;
        }
        public static Bitmap setRGBData(int[,,] rgbData)
        {
            Bitmap bitImg;
            int width = rgbData.GetLength(0);
            int height = rgbData.GetLength(1);
            bitImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// 24bit per pixel 8x8x8
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //get image starting place
            IntPtr imgPtr = bitmapData.Scan0;
            //image scan width
            int stride = bitmapData.Stride;
            int widthByte = width * 3;
            int skipByte = stride - widthByte;
            #region
            unsafe
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        p[0] = (byte)rgbData[i, j, 2];
                        p++;
                        p[0] = (byte)rgbData[i, j, 1];
                        p++;
                        p[0] = (byte)rgbData[i, j, 0];
                        p++;
                    }
                    p += skipByte;
                }

            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return bitImg;
        }



    }
}
