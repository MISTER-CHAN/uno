using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Uno
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Debugger.IsAttached)
            {
                string s = Application.StartupPath;
                s = s.Substring(0, s.Length - 9) + "Properties\\AssemblyInfo.cs";

                // 检测AssemblyInfo文件是否存在来决定路径是否设置正确
                if (!System.IO.File.Exists(s))
                {
                    MessageBox.Show("檔案 " + s + " 不存在!", "偵錯", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                // TODO: 自行检测文件是否只读等问题
                System.IO.StreamReader sr = new System.IO.StreamReader(s);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                    s + ".bak",
                    false,
                    sr.CurrentEncoding);

                string line;
                string newLine;
                while ((line = sr.ReadLine()) != null)
                {
                    if (new Regex("^\\w*\\[assembly: Assembly(File)?Version").IsMatch(line))
                    {
                        // 找到这两行了，修改它们
                        newLine = line.Substring(0, line.IndexOf("(\"") + 2)
                            + DateTime.Now.Year + "."
                            + DateTime.Now.Month.ToString("00") + "."
                            + DateTime.Now.Day.ToString("00") + "\")]";
                    }
                    else
                    {
                        newLine = line;
                    }

                    sw.WriteLine(newLine);
                }
                sw.Close();
                sr.Close();

                // 新文件改为原文件（原只读属性将会丢失）
                System.IO.File.Delete(s);
                System.IO.File.Move(s + ".bak", s);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Options());
        }
    }
}
