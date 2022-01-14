using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Threading;



namespace HotVHDPEDeployTool
{
    public partial class Form1 : Form
    {




        //调用dll
        [DllImport("kernel32.dll", EntryPoint = "GetLogicalDriveStringsA")]
        static extern int GetLogicalDriveStrings(int nBufferLength, byte lpBuffer);


        //定义函数

        public static string GetLeft(string str, string s)//取文本左边，s标识符
        {
            string temp = str.Substring(0, str.IndexOf(s));
            return temp;
        }
        private static bool IfBeLetter(string letter)//盘符是否存在
        {
            System.IO.DriveInfo[] disk = System.IO.DriveInfo.GetDrives();//取所有盘符
            foreach (System.IO.DriveInfo di in disk)
            {
                if (GetLeft(di.Name,":") == letter)
                {
                    return true ;
                }
            }
            return false;
        }
        private static string GetUsableLetter()//取可用盘符
        {
            if (IfBeLetter("C") != true) { return "C";}
            if (IfBeLetter("D") != true) { return "D"; }
            if (IfBeLetter("E") != true) { return "E"; }
            if (IfBeLetter("F") != true) { return "F"; }
            if (IfBeLetter("G") != true) { return "G"; }
            if (IfBeLetter("H") != true) { return "H"; }
            if (IfBeLetter("I") != true) { return "I"; }
            if (IfBeLetter("J") != true) { return "J"; }
            if (IfBeLetter("K") != true) { return "K"; }
            if (IfBeLetter("L") != true) { return "L"; }
            if (IfBeLetter("M") != true) { return "M"; }
            if (IfBeLetter("N") != true) { return "N"; }
            if (IfBeLetter("O") != true) { return "O"; }
            if (IfBeLetter("P") != true) { return "P"; }
            if (IfBeLetter("Q") != true) { return "Q"; }
            if (IfBeLetter("R") != true) { return "R"; }
            if (IfBeLetter("S") != true) { return "S"; }
            if (IfBeLetter("T") != true) { return "T"; }
            if (IfBeLetter("U") != true) { return "U"; }
            if (IfBeLetter("V") != true) { return "V"; }
            if (IfBeLetter("W") != true) { return "W"; }
            if (IfBeLetter("X") != true) { return "X"; }
            if (IfBeLetter("Y") != true) { return "Y"; }
            if (IfBeLetter("Z") != true) { return "Z"; }
            if (IfBeLetter("B") != true) { return "B"; }
            if (IfBeLetter("A") != true) { return "A"; }
            return "";      //输出出流取得命令行结果果
        }
        private static string RunCmd(string command)//运行cmd命令
        {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";         //确定程序名
                p.StartInfo.Arguments = "/c " + command;   //确定程式命令行
                p.StartInfo.UseShellExecute = false;      //Shell的使用
                p.StartInfo.RedirectStandardInput = true;  //重定向输入
                p.StartInfo.RedirectStandardOutput = true; //重定向输出
                p.StartInfo.RedirectStandardError = true;  //重定向输出错误
                p.StartInfo.CreateNoWindow = true;        //设置置不显示示窗口
                p.Start();
                return p.StandardOutput.ReadToEnd();      //输出出流取得命令行结果果
        }
        private static string LoadVHDX(string path, string letter, string label)//加载vhdx
        {
            string str = (((("SELECT VDISK FILE=\"" + path + "\"") + "\nattach vdisk" + "\ncreate partition primary") + "\nFORMAT FS=NTFS LABEL=\"" + label + "\" QUICK ") + "\nassign letter = " + letter) + "\nactive" + "\nexit";
            string tempPath = Path.GetTempPath();
            File.Delete(tempPath + "LoadVHDX.txt");
            StreamWriter writer = new StreamWriter(tempPath + "LoadVHDX.txt");
            writer.WriteLine(str);
            writer.Flush();
            writer.Close();
            return RunCmd("diskpart /s " + tempPath + "LoadVHDX.txt");
        }
        private static string SetVHDX(string path, int size, string type)//创建vhdx
        {
            string diskpartScript = "create vdisk file=" + path + " maximum=" + size + " type=" + type;
            string TempPath = System.IO.Path.GetTempPath();
            System.IO.File.Delete(@TempPath + "setVHD.txt");//删除文件
            StreamWriter File = new StreamWriter(@TempPath + "setVHD.txt");
            File.WriteLine(diskpartScript);
            File.Flush();//刷新缓存
            File.Close();//关闭流
            string output = RunCmd(@"diskpart /s " + TempPath + "setVHD.txt");
            return output;
        }
        private static string UnloadVHDX(string path)//卸载vhdx
        {
            string str = ("SELECT VDISK FILE=\"" + path + "\"") + "\n detach vdisk" + "\nexit";
            string tempPath = Path.GetTempPath();
            File.Delete(tempPath + "UnloadVHDX.txt");
            StreamWriter writer = new StreamWriter(tempPath + "UnloadVHDX.txt");
            writer.WriteLine(str);
            writer.Flush();
            writer.Close();
            return RunCmd("diskpart /s " + tempPath + "UnloadVHDX.txt");
        }
        private static string InitializeUSB(int number, string letter, string label)//初始化U盘
        {
            string str = ("select disk " + Convert.ToString(number) ) + "\n clean" + "\nconvert mbr" + "\ncreate partition primary" + "\nFORMAT FS=NTFS LABEL=\"" + label + "\" QUICK " + "\nassign letter = " + letter + "\nactive" + "\nexit";
            string tempPath = Path.GetTempPath();
            File.Delete(tempPath + "InitializeUSB.txt");
            StreamWriter writer = new StreamWriter(tempPath + "InitializeUSB.txt");
            writer.WriteLine(str);
            writer.Flush();
            writer.Close();
            return RunCmd("diskpart /s " + tempPath + "InitializeUSB.txt");
        }

        private static bool WriteToFile(byte[] data, string path)//写到文件
        {
            FileStream fs = new FileStream(path , FileMode.Create);//实例化一个文件流--->与写入文件相关联  
            fs.Write(data, 0, data.Length);//开始写入  
            fs.Flush();//清空缓冲区、关闭流  
            fs.Close();
            return File.Exists(path);
        }


        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {

            Task t = new Task(MainFunction);
            t.Start();

        }


        private void MainFunction()
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("请填写正确参数！", "信息");
                return;
            }
            if (!System.IO.File.Exists(textBox3.Text))
            {
                MessageBox.Show("请填写正确参数！", "信息");
                return;
            }
            if (checkBox1.Checked)
            {
                if (!IfBeLetter(textBox2.Text.ToUpper()))
                {
                    MessageBox.Show("请填写正确参数！", "信息");
                    return;
                }
            }
            else
            {
                if (textBox2.Text == "")
                {
                    MessageBox.Show("请填写正确参数！", "信息");
                    return;
                }
            }


            if (!checkBox1.Checked)
            {
                MessageBox.Show("此操作会清空U盘，请备份好U盘中的数据！", "警告");
            }

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            comboBox1.Enabled = false;
            checkBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            progressBar1.Value = 3;
            string typestr;
            if (this.comboBox1.SelectedIndex == 0)//type=expandable:动态大小 fixed:固定大小
            {
                typestr = "expandable";
            }
            else
            {
                typestr = "fixed";
            }

            string USBLetter = "";
            string VHDXpath = "";

            if (!checkBox1.Checked)
            {
                USBLetter = GetUsableLetter();
                VHDXpath = USBLetter + ":\\HotVHDPE.vhdx";
                InitializeUSB(Convert.ToInt32(textBox2.Text), USBLetter, "HotVHDPE");//初始化U盘
                if (!IfBeLetter(USBLetter))
                {
                    MessageBox.Show("部署失败！！！请确认U盘没被其他程序占用，建议插拔U盘或重启再试！", "部署失败");
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    comboBox1.Enabled = true;
                    checkBox1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    progressBar1.Value = 0;
                    return;
                }
                progressBar1.Value = 9;
            }
            else
            {
                USBLetter = textBox2.Text.ToUpper();
                VHDXpath = USBLetter + ":\\HotVHDPE.vhdx";
                UnloadVHDX(VHDXpath);//卸载VHDX
                File.Delete(VHDXpath);
                progressBar1.Value = 9;
            }
            RunCmd("xcopy " + Path.GetTempPath() + "\\HotVHDPE\\" + "vhdboot\\inUSB\\* " + USBLetter + ":\\    /E  /H /R /Y");//复制引导文件usb
            progressBar1.Value = 12;
            SetVHDX(VHDXpath, Convert.ToInt32(textBox1.Text) * 1024, typestr);//创建VHDX
            progressBar1.Value = 30;
            RunCmd("ATTRIB  +S +H " + USBLetter + ":\\* / D / S");//文件属性
            string VHDXLetter = GetUsableLetter();
            LoadVHDX(VHDXpath, VHDXLetter, "HotVHDPE"); //加载VHDX
            if (!IfBeLetter(VHDXLetter))
            {
                MessageBox.Show("部署失败！！！请确认U盘没被其他程序占用，建议插拔U盘或重启再试！", "部署失败");
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                comboBox1.Enabled = true;
                checkBox1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                progressBar1.Value = 0;
                return;
            }
            progressBar1.Value = 38;
            RunCmd("xcopy " + Path.GetTempPath() + "\\HotVHDPE\\" + "vhdboot\\inVHD\\* " + VHDXLetter + ":\\   /E  /H /R /Y");//复制引导文件vhdx
            RunCmd("ATTRIB  +S +H " + VHDXLetter + ":\\* / D / S");//文件属性
            progressBar1.Value = 40;
            RunCmd(Path.GetTempPath() + "\\HotVHDPE\\" + "wimlib_imagex.exe" + " apply " + "\"" + textBox3.Text + "\" " + textBox4.Text + " " + VHDXLetter + ":\\");
            progressBar1.Value = 90;
            UnloadVHDX(VHDXpath);//卸载VHDX
            progressBar1.Value = 100;
            MessageBox.Show("部署完成！", "信息");
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            comboBox1.Enabled = true;
            checkBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            progressBar1.Value = 0;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            string tempPath = Path.GetTempPath();
            string ToolPath = tempPath + "\\HotVHDPE\\";


            if (Directory.Exists(ToolPath))
            {
                DirectoryInfo di = new DirectoryInfo(ToolPath);
                di.Delete(true);//删除目录
            }
            Directory.CreateDirectory(ToolPath);//创建目录
            //释放文件
            WriteToFile(Properties.Resources.BOOTICE, ToolPath+ "BOOTICE.exe");
            WriteToFile(Properties.Resources.libwim_15, ToolPath + "libwim-15.dll");
            WriteToFile(Properties.Resources.wimlib_imagex, ToolPath + "wimlib_imagex.exe");
            WriteToFile(Properties.Resources.wimlib_imagex, ToolPath + "wimlib_imagex.exe");
            WriteToFile(Properties.Resources.VHDXBootFile, ToolPath + "VHDXBootFile.ZIP");
            // 解压引导文件
            ZipFile.ExtractToDirectory(ToolPath + "VHDXBootFile.ZIP", ToolPath);

            this.comboBox1.Items.Add("动态扩展");
            this.comboBox1.Items.Add("固定大小");
            this.comboBox1.SelectedIndex = 0;

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.hotpe.top/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://space.bilibili.com/443230923");

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://znpe.sys-img.top/");
        }



        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择镜像文件";
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//这是系统提供的桌面路径
            ofd.Filter = "(*.wim)|*.wim|(*.esd)|*.esd";//过滤不同类型的文件
            if (ofd.ShowDialog()== DialogResult.OK)
            {
                textBox3.Text = ofd.FileName;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label2.Text = "安装盘符(字母):";
                textBox2.Text = "";
            }
            else
            {
                label2.Text = "安装U盘(序号):";
                textBox2.Text = "";
            }
        }
    }
}



