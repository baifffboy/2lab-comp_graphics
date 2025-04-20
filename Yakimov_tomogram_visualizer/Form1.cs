using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Yakimov_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        bool r = false;
        bool m = false;
        bool s = false;
        Bin a = new Bin();
        bool expectation = false;
        View view = new View();
        int currentLayer = 0; // текущий слой томограммы, регулируется трэкбаром
        DateTime NextFPSUpdate; // больше на 1 секунду, чем текущее время, когда настоящее время догоняет, становится равным ему + 1
        int FrameCount; // количество кадров отрисовки (fps)
        bool needReload = false; // флаг - переключатель каким образом отрисовываем томограмму

        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;

        }

        private void button1_Click(object sender, EventArgs e) // считывание томограммы из .bin файла
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK) { 
                string str = dialog.FileName;
                a.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true; //loaded = true после открытия файла
                glControl1.Invalidate();

            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e) // отрисовка томограммы loaded = true после открытия файла
        {
            if (loaded) 
            {
                if (expectation) {
                    glControl1.BackColor = System.Drawing.Color.Black;
                }
                if (m)
                {
                    for (int i = 0; i < currentLayer; i++)
                    {
                        if (needReload)
                        {
                            view.generateTextureImage(i);
                            view.Load2DTexture();
                        }
                        if (expectation) break;
                        view.DrawTexture();
                        glControl1.SwapBuffers();    
                    }
                }
                else {
                    if (s == false)
                    {
                        if (r == false)
                        {
                            for (int i = 0; i < currentLayer; i++)
                            {
                                if (expectation) break;
                                view.DrawQuads(i , r);
                                glControl1.SwapBuffers();
                            }
                        }
                        else {
                            for (int i = 0; i < currentLayer; i++)
                            {
                                if (expectation) break;
                                view.DrawQuads(i, r);
                                glControl1.SwapBuffers();
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < currentLayer; i++)
                        {
                            if (expectation) break;
                            view.DrawQuadStrip(i);
                            glControl1.SwapBuffers();
                        }
                    }
                }
            }
        }   

        private void trackBar1_Scroll(object sender, EventArgs e) 
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle1;
        }

        private void Application_Idle1(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                DisplayFPS();
                glControl1.Invalidate();
            }
        }

        
        void DisplayFPS() {
            
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualiser (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            needReload = false;
            m = false;
            expectation = false;
            s = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            needReload = true;
            m = true;
            expectation = false;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.min = trackBar2.Value;
            if (radioButton1.Checked) {
                needReload = false;
                m = false;
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.width = trackBar3.Value;
            if (radioButton2.Checked)
            {
                needReload = true;
                m = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            expectation = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            expectation = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            needReload = false;
            m = false;
            expectation = false;
            s = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { r = true; }
            else { r = false; }
        }
    }
}
