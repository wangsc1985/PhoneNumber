using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace phoneNumber
{
    /* 四吉星：生气     延年     天医     伏位
     * 四凶星：绝命     五鬼     六煞     祸害
     * 生气：财运大好，身体健康，活力充沛。
     * 延年：财运很好，延年益寿，身体健康。
     * 天医：财运不错，疾病痊愈，贵人相助。
     * 伏位：财运小吉，运气中等，健康如常。
     * 绝命：财运极差，多病损寿，凶则死亡。
     * 五鬼：破财连连，身体甚差，容易招鬼。
     * 六煞：财运不好，灾祸连连，身体多病。
     * 祸害：财难积聚，官灾是非，争执被骗
     */
    public partial class MainForm : Form
    {
        private bool isOpen姓名学测法;
        private string ns = "";
        private delegate void FormControlInvoker();
        private Thread th;
        public MainForm()
        {
            InitializeComponent();
            //var tmp = this.GetRegValue(RegKey, RegName姓名学测法);
            //if (tmp != null)
            //{
            //    isOpen姓名学测法 = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName大吉);
            //if (tmp != null)
            //{
            //    this.checkBox大吉.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName吉带凶);
            //if (tmp != null)
            //{
            //    this.checkBox吉带凶.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName凶带吉);
            //if (tmp != null)
            //{
            //    this.checkBox凶带吉.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName凶);
            //if (tmp != null)
            //{
            //    this.checkBox凶.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName生气);
            //if (tmp != null)
            //{
            //    this.checkBox生气.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName延年);
            //if (tmp != null)
            //{
            //    this.checkBox延年.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName天医);
            //if (tmp != null)
            //{
            //    this.checkBox天医.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName小天医);
            //if (tmp != null)
            //{
            //    this.checkBox小天医.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName伏位);
            //if (tmp != null)
            //{
            //    this.checkBox伏位.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName绝命);
            //if (tmp != null)
            //{
            //    this.checkBox绝命.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName五鬼);
            //if (tmp != null)
            //{
            //    this.checkBox五鬼.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName六煞);
            //if (tmp != null)
            //{
            //    this.checkBox六煞.Checked = bool.Parse(tmp);
            //}
            //tmp = this.GetRegValue(RegKey, RegName祸害);
            //if (tmp != null)
            //{
            //    this.checkBox祸害.Checked = bool.Parse(tmp);
            //}

        }

        private void button统计_Click(object sender, EventArgs e)
        {
            textBox内容.Visible = false;
            if (button统计.Text.Equals("终止"))
            {
                if (th != null)
                {
                    th.Abort();
                    ThreadEnd();
                }
            }
            if (!string.IsNullOrEmpty(textBox内容.Text))
            {
                this.listBox1.Items.Clear();
                ns = textBox内容.Text;
                th = new Thread(new ThreadStart(PerseNumbers));
                th.Start();
            }
        }
        private void button导出_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0)
            {
                MessageBox.Show("列表中没有数据！");
                return;
            }
            var fileName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\手机号码.", DateTime.Now.ToString("MMdd.HHmm.ss"), ".txt");
            var file = new StreamWriter(File.OpenWrite(fileName));
            foreach (var item in this.listBox1.Items)
            {
                file.WriteLine(item.ToString());
            }
            file.Close();
            MessageBox.Show(string.Concat("数据已保存：", fileName));
        }

        private void PerseNumbers()
        {
            List<string> numbers = new List<string>();

            Regex reg = new Regex("[0-9]{11}");
            var collection = reg.Matches(ns);

            foreach (var num in collection)
            {
                numbers.Add(num.ToString());
            }


            ThreadStart(numbers);
            List<string> list = new List<string>();
            foreach (var number in numbers)
            {
                // 
                this.Invoke(new FormControlInvoker(() =>
                {
                    if (isOpen姓名学测法)
                    {
                        string summary = name(number);
                        if (checkBox大吉.Checked && !summary.Contains("凶带吉") && !summary.Contains("吉带凶") && summary.Contains("吉"))
                        {
                            var baShen = WuXing(number);
                            if (!((checkBox生气.Checked && !baShen.Contains(checkBox生气.Text))
                                || (checkBox延年.Checked && !baShen.Contains(checkBox延年.Text))
                                || (checkBox天医.Checked && !baShen.Contains(checkBox天医.Text))
                                || (checkBox小天医.Checked && !baShen.Contains(checkBox小天医.Text) && !baShen.Contains(checkBox天医.Text))
                                || (checkBox伏位.Checked && !baShen.Contains(checkBox伏位.Text))
                                || (checkBox绝命.Checked && baShen.Contains(checkBox绝命.Text))
                                || (checkBox五鬼.Checked && baShen.Contains(checkBox五鬼.Text))
                                || (checkBox六煞.Checked && baShen.Contains(checkBox六煞.Text))
                                || (checkBox祸害.Checked && baShen.Contains(checkBox祸害.Text))
                                ))
                            {
                                this.listBox1.Items.Add(number);
                                this.listBox1.Items.Add(summary);
                                //this.listBox1.Items.Add(details);
                                //this.listBox1.Items.Add("吉");
                                this.listBox1.Items.Add(baShen);
                                this.listBox1.Items.Add("");
                            }
                        }
                        else if (checkBox吉带凶.Checked && summary.Contains("吉带凶"))
                        {
                            var baShen = WuXing(number);
                            if (!((checkBox生气.Checked && !baShen.Contains(checkBox生气.Text))
                                || (checkBox延年.Checked && !baShen.Contains(checkBox延年.Text))
                                || (checkBox天医.Checked && !baShen.Contains(checkBox天医.Text))
                                || (checkBox小天医.Checked && !baShen.Contains(checkBox小天医.Text) && !baShen.Contains(checkBox天医.Text))
                                || (checkBox伏位.Checked && !baShen.Contains(checkBox伏位.Text))
                                || (checkBox绝命.Checked && baShen.Contains(checkBox绝命.Text))
                                || (checkBox五鬼.Checked && baShen.Contains(checkBox五鬼.Text))
                                || (checkBox六煞.Checked && baShen.Contains(checkBox六煞.Text))
                                || (checkBox祸害.Checked && baShen.Contains(checkBox祸害.Text))
                                ))
                            {
                                this.listBox1.Items.Add(number);
                                this.listBox1.Items.Add(summary);
                                //this.listBox1.Items.Add(details);
                                //this.listBox1.Items.Add("吉带凶");
                                this.listBox1.Items.Add(baShen);
                                this.listBox1.Items.Add("");
                            }
                        }
                        else if (checkBox凶带吉.Checked && summary.Contains("凶带吉"))
                        {
                            var baShen = WuXing(number);
                            if (!((checkBox生气.Checked && !baShen.Contains(checkBox生气.Text))
                                || (checkBox延年.Checked && !baShen.Contains(checkBox延年.Text))
                                || (checkBox天医.Checked && !baShen.Contains(checkBox天医.Text))
                                || (checkBox小天医.Checked && !baShen.Contains(checkBox小天医.Text) && !baShen.Contains(checkBox天医.Text))
                                || (checkBox伏位.Checked && !baShen.Contains(checkBox伏位.Text))
                                || (checkBox绝命.Checked && baShen.Contains(checkBox绝命.Text))
                                || (checkBox五鬼.Checked && baShen.Contains(checkBox五鬼.Text))
                                || (checkBox六煞.Checked && baShen.Contains(checkBox六煞.Text))
                                || (checkBox祸害.Checked && baShen.Contains(checkBox祸害.Text))
                                ))
                            {
                                this.listBox1.Items.Add(number);
                                this.listBox1.Items.Add(summary);
                                //this.listBox1.Items.Add(details);
                                //this.listBox1.Items.Add("吉带凶");
                                this.listBox1.Items.Add(baShen);
                                this.listBox1.Items.Add("");
                            }
                        }
                        else if (checkBox凶.Checked)
                        {
                            var baShen = WuXing(number);
                            if (!((checkBox生气.Checked && !baShen.Contains(checkBox生气.Text))
                                || (checkBox延年.Checked && !baShen.Contains(checkBox延年.Text))
                                || (checkBox天医.Checked && !baShen.Contains(checkBox天医.Text))
                                || (checkBox小天医.Checked && !baShen.Contains(checkBox小天医.Text) && !baShen.Contains(checkBox天医.Text))
                                || (checkBox伏位.Checked && !baShen.Contains(checkBox伏位.Text))
                                || (checkBox绝命.Checked && baShen.Contains(checkBox绝命.Text))
                                || (checkBox五鬼.Checked && baShen.Contains(checkBox五鬼.Text))
                                || (checkBox六煞.Checked && baShen.Contains(checkBox六煞.Text))
                                || (checkBox祸害.Checked && baShen.Contains(checkBox祸害.Text))
                                ))
                            {
                                this.listBox1.Items.Add(number);
                                this.listBox1.Items.Add(summary);
                                //this.listBox1.Items.Add(details);
                                //this.listBox1.Items.Add("【凶】");

                                this.listBox1.Items.Add(baShen);
                                this.listBox1.Items.Add("");
                            }
                        }
                    }
                    else
                    {
                        var baShen = WuXing(number);
                        if (!((checkBox生气.Checked && !baShen.Contains(checkBox生气.Text))
                            || (checkBox延年.Checked && !baShen.Contains(checkBox延年.Text))
                            || (checkBox天医.Checked && !baShen.Contains(checkBox天医.Text))
                            || (checkBox小天医.Checked && !baShen.Contains(checkBox小天医.Text) && !baShen.Contains(checkBox天医.Text))
                            || (checkBox伏位.Checked && !baShen.Contains(checkBox伏位.Text))
                            || (checkBox绝命.Checked && baShen.Contains(checkBox绝命.Text))
                            || (checkBox五鬼.Checked && baShen.Contains(checkBox五鬼.Text))
                            || (checkBox六煞.Checked && baShen.Contains(checkBox六煞.Text))
                            || (checkBox祸害.Checked && baShen.Contains(checkBox祸害.Text))
                            ))
                        {
                            this.listBox1.Items.Add(number);
                            this.listBox1.Items.Add(baShen);
                            this.listBox1.Items.Add("");
                        }
                    }
                    this.listBox1.Refresh();
                }));
                this.Invoke(new FormControlInvoker(() =>
                {
                    progressBar1.Value++;
                }));
            }
            ThreadEnd();
        }

        public string name(string number)
        {

            decimal num = decimal.Parse(number.Substring(7));
            num = num / 80;
            num = num % 1;
            num = num * 80;

            List<string> result = new List<string>();
            result.Add("0 占位");
            result.Add("1 大展鸿图．可获成功 吉");
            result.Add("2 一盛一衰．劳而无功 凶");
            result.Add("3 蒸蒸日上．百事顺遂 吉");
            result.Add("4 坎坷前途．苦难折磨 凶");
            result.Add("5 生意欣荣．名利双收 吉");
            result.Add("6 天降幸运．可成大功 吉");
            result.Add("7 和气致祥．必获成功 吉");
            result.Add("8 贯彻志望．成功可期 吉");
            result.Add("9 独营无力．财利无望 凶");
            result.Add("10 空费心力．徒劳无功 凶");
            result.Add("11 稳健着实．必得人望 吉");
            result.Add("12 薄弱无力．谋事难成 凶");
            result.Add("13 天赋吉运．能得人望 吉");
            result.Add("14 是成是败．惟靠坚毅 凶");
            result.Add("15 大事成就．一定兴隆 吉");
            result.Add("16 成就大业．名利双收 吉");
            result.Add("17 有贵人助．可得成功 吉");
            result.Add("18 顺利昌隆．百事亨通 吉");
            result.Add("19 内外不合．障碍重重 凶");
            result.Add("20 历尽艰难．焦心忧劳 凶");
            result.Add("21 专心经营．善用智能 吉");
            result.Add("22 怀才不遇．事不如意 凶");
            result.Add("23 名显四方．终成大业 吉");
            result.Add("24 须靠自力．能奏大功 吉");
            result.Add("25 天时地利．再得人格 吉");
            result.Add("26 波澜起伏．凌驾万难 凶");
            result.Add("27 一盛一衰．可守成功 凶带吉");
            result.Add("29 青云直上．才略奏功 吉");
            result.Add("30 吉凶参半．得失相伴 凶");
            result.Add("31 名利双收．大业成就 吉");
            result.Add("32 池中之龙．成功可望 吉");
            result.Add("33 智能慎始．必可昌隆 ");
            result.Add("34 灾难不绝．难望成功 凶");
            result.Add("35 中吉之数．进退保守 ");
            result.Add("36 波澜重叠．常陷穷困 ");
            result.Add("37 逢凶化吉．风调雨顺 吉");
            result.Add("38 名虽可得．利则难获 凶带吉");
            result.Add("39 光明坦途．指日可待 吉");
            result.Add("40 一盛一衰．浮沉不定 吉带凶");
            result.Add("41 天赋吉运．前途无限 吉");
            result.Add("42 事业不专．十九不成 吉带凶");
            result.Add("43 忍耐自重．转凶为吉 吉带凶");
            result.Add("44 事难遂愿．贪功好进 凶");
            result.Add("45 绿叶发枝．一举成名 吉");
            result.Add("46 坎坷不平．艰难重重 凶");
            result.Add("47 有贵人助．可成大业 吉");
            result.Add("48 名利俱全．繁荣富贵 吉");
            result.Add("49 遇吉则吉．遇凶则凶 凶");
            result.Add("50 吉凶互见．一成一败 吉带凶");
            result.Add("51 一盛一衰．浮沉不常 吉带凶");
            result.Add("52 雨过天青．即获成功 吉");
            result.Add("53 盛衰参半．先吉后凶 吉带凶");
            result.Add("54 虽倾全力．难望成功 凶");
            result.Add("55 外观隆昌．内隐祸患 吉带凶");
            result.Add("56 事与愿违．终难成功 凶");
            result.Add("57 努力经营．时来运转 吉");
            result.Add("58 浮沉多端．始凶终吉 凶带吉");
            result.Add("59 遇事犹疑．难望成事 凶");
            result.Add("60 心迷意乱．难定方针 凶");
            result.Add("61 云遮半月．百隐风波 吉带凶");
            result.Add("62 烦闷懊恼．事事难展 凶");
            result.Add("63 万物化育．繁荣之象 吉");
            result.Add("64 十九不成．徒劳无功 凶");
            result.Add("65 吉运自来．能享盛名 吉");
            result.Add("66 内外不和．信用缺乏 凶");
            result.Add("67 事事如意．富贵自来 吉");
            result.Add("68 不失先机．可望成功 吉");
            result.Add("69 动摇不安．常陷逆境 凶");
            result.Add("70 惨淡经营．难免贫困 凶");
            result.Add("71 吉凶参半．惟赖勇气 吉带凶");
            result.Add("72 得而复失．难以安顺 凶");
            result.Add("73 安乐自来．自然吉祥 吉");
            result.Add("74 如无智谋．难望成功 凶");
            result.Add("75 吉中带凶．进不如守 吉带凶");
            result.Add("76 此数大凶．破产之象 凶");
            result.Add("77 先苦后甘．不致失败 吉带凶");
            result.Add("78 有得有失．华而不实 吉带凶");
            result.Add("79 前途无光．希望不大 ");
            result.Add("80 得而复失．枉费心机 吉带凶");
            result.Add("81 最极之数．能得成功 吉");
            return result.ElementAt((int)num);
        }


        private void ThreadStart(List<string> numbers)
        {
            this.Invoke(new FormControlInvoker(() =>
            {
                this.button导出.Enabled = false;
                this.checkBox生气.Enabled = false;
                this.checkBox延年.Enabled = false;
                this.checkBox天医.Enabled = false;
                this.checkBox小天医.Enabled = false;
                this.checkBox伏位.Enabled = false;
                this.checkBox绝命.Enabled = false;
                this.checkBox五鬼.Enabled = false;
                this.checkBox六煞.Enabled = false;
                this.checkBox祸害.Enabled = false;
                this.checkBox大吉.Enabled = false;
                this.checkBox吉带凶.Enabled = false;
                this.checkBox凶带吉.Enabled = false;
                this.checkBox凶.Enabled = false;
                this.button统计.Text = "终止";

                progressBar1.Height = 10;
                progressBar1.Maximum = numbers.Count();
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
            }));
        }

        private void ThreadEnd()
        {
            this.Invoke(new FormControlInvoker(() =>
            {
                this.checkBox生气.Enabled = true;
                this.checkBox延年.Enabled = true;
                this.checkBox天医.Enabled = true;
                this.checkBox小天医.Enabled = true;
                this.checkBox伏位.Enabled = true;
                this.checkBox绝命.Enabled = true;
                this.checkBox五鬼.Enabled = true;
                this.checkBox六煞.Enabled = true;
                this.checkBox祸害.Enabled = true;
                this.checkBox大吉.Enabled = true;
                this.checkBox吉带凶.Enabled = true;
                this.checkBox凶带吉.Enabled = true;
                this.checkBox凶.Enabled = true;
                this.button导出.Enabled = true;
                this.button统计.Text = "统计";
                this.button统计.Enabled = true;
                progressBar1.Value = 0;
                progressBar1.Height = 0;
            }));
        }

        /// <summary>
        /// 得到指定手机号八神吉凶。
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string WuXing(string number)
        {
            StringBuilder result = new StringBuilder();
            char preNum = 'E';
            string midNum = "";
            string midStr = "";
            string tmp = "";
            for (int i = 0; i < 11; i++)
            {
                var currentNum = number[i];
                tmp += currentNum;
                switch (currentNum)
                {
                    case '0':
                        midNum += "0";
                        midStr += "隐";
                        break;
                    case '5':
                        midNum += "5";
                        midStr += "显";
                        break;
                    default:
                        if (preNum != 'E')
                        {
                            string subnum = string.Concat(preNum, currentNum);
                            result.Append(string.Concat(preNum, midNum, currentNum) + GuiWei(subnum, midStr));
                            tmp = "";
                            midStr = "";
                        }
                        midNum = "";
                        preNum = currentNum;
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 得到2位数八神所属。
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        private string GuiWei(string sub, string midStr)
        {
            switch (sub)
            {
                case "11":
                case "22":
                case "33":
                case "44":
                case "66":
                case "77":
                case "88":
                case "99": return string.Concat("《", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox伏位.Text, "》");
                case "26":
                case "62":
                case "34":
                case "43":
                case "78":
                case "87":
                case "19":
                case "91": return string.Concat("《", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox延年.Text, "》");
                case "14":
                case "41":
                case "28":
                case "82":
                case "67":
                case "76":
                case "39":
                case "93": return string.Concat("《", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox生气.Text, "》");
                case "13":
                case "31":
                case "68":
                case "86": return string.Concat("《", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox天医.Text, "》");
                case "27":
                case "72":
                case "49":
                case "94": return string.Concat("《", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox小天医.Text, "》");
                case "17":
                case "71":
                case "23":
                case "32":
                case "46":
                case "64":
                case "89":
                case "98": return string.Concat("【", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox祸害.Text, "】");
                case "16":
                case "61":
                case "38":
                case "83":
                case "47":
                case "74":
                case "29":
                case "92": return string.Concat("【", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox六煞.Text, "】");
                case "12":
                case "21":
                case "37":
                case "73":
                case "48":
                case "84":
                case "69":
                case "96": return string.Concat("【", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox绝命.Text, "】");
                case "18":
                case "81":
                case "24":
                case "42":
                case "36":
                case "63":
                case "79":
                case "97": return string.Concat("【", string.IsNullOrEmpty(midStr) ? "" : midStr + "-", checkBox五鬼.Text, "】");
            }
            return "        ";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        private string RegKey = "手机号码";
        private string RegName姓名学测法 = "姓名学测法";
        private string RegName大吉 = "大吉";
        private string RegName吉带凶 = "吉带凶";
        private string RegName凶带吉= "凶带吉";
        private string RegName凶 = "凶";
        private string RegName生气 = "生气";
        private string RegName延年 = "延年";
        private string RegName天医 = "天医";
        private string RegName小天医 = "小天医";
        private string RegName伏位 = "伏位";
        private string RegName绝命 = "绝命";
        private string RegName五鬼 = "五鬼";
        private string RegName六煞 = "六煞";
        private string RegName祸害 = "祸害";

        /// <summary>
        /// 写入注册表,如果指定项已经存在,则修改指定项的值
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <param name="values">值</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        //private void SetRegValue(string key, string name, string value)
        //{
        //    try
        //    {
        //        RegistryKey hklm = Registry.LocalMachine;
        //        RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
        //        RegistryKey aimdir = software.CreateSubKey(key);
        //        aimdir.SetValue(name, value);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        private void checkBox大吉_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox吉带凶.Checked || checkBox凶带吉.Checked || checkBox大吉.Checked || checkBox凶.Checked)
                isOpen姓名学测法 = true;
            else
                isOpen姓名学测法 = false;
            //this.SetRegValue(RegKey, RegName姓名学测法, isOpen姓名学测法.ToString());

            //this.SetRegValue(RegKey, RegName大吉, checkBox大吉.Checked.ToString());
        }

        private void checkBox吉带凶_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox吉带凶.Checked || checkBox凶带吉.Checked || checkBox大吉.Checked || checkBox凶.Checked)
                isOpen姓名学测法 = true;
            else
                isOpen姓名学测法 = false;
            //this.SetRegValue(RegKey, RegName姓名学测法, isOpen姓名学测法.ToString());

            //this.SetRegValue(RegKey, RegName吉带凶, checkBox吉带凶.Checked.ToString());
        }

        private void checkBox凶带吉_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox吉带凶.Checked || checkBox凶带吉.Checked || checkBox大吉.Checked || checkBox凶.Checked)
                isOpen姓名学测法 = true;
            else
                isOpen姓名学测法 = false;
            //this.SetRegValue(RegKey, RegName姓名学测法, isOpen姓名学测法.ToString());

            //this.SetRegValue(RegKey, RegName凶带吉, checkBox凶带吉.Checked.ToString());
        }
        private void checkBox凶_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox吉带凶.Checked || checkBox凶带吉.Checked || checkBox大吉.Checked || checkBox凶.Checked)
                isOpen姓名学测法 = true;
            else
                isOpen姓名学测法 = false;
            //this.SetRegValue(RegKey, RegName姓名学测法, isOpen姓名学测法.ToString());

            //this.SetRegValue(RegKey, RegName凶, checkBox凶.Checked.ToString());
        }

        /// <summary>
        /// 读取注册表
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <returns>返回字符串</returns>
        private string GetRegValue(string key, string name)
        {
            try
            {
                string registData = null;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey(key, true);
                if (aimdir != null)
                {
                    var obj = aimdir.GetValue(name);
                    registData = obj == null ? null : obj.ToString();
                }
                return registData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void checkBox生气_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName生气, checkBox生气.Checked.ToString());
        }

        private void checkBox延年_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName延年, checkBox延年.Checked.ToString());
        }

        private void checkBox天医_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName天医, checkBox天医.Checked.ToString());
        }

        private void checkBox伏位_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName伏位, checkBox伏位.Checked.ToString());
        }

        private void checkBox绝命_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName绝命, checkBox绝命.Checked.ToString());
        }

        private void checkBox五鬼_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName五鬼, checkBox五鬼.Checked.ToString());
        }

        private void checkBox六煞_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName六煞, checkBox六煞.Checked.ToString());
        }

        private void checkBox祸害_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName祸害, checkBox祸害.Checked.ToString());
        }

        private void checkBox小天医_CheckedChanged(object sender, EventArgs e)
        {
            //this.SetRegValue(RegKey, RegName小天医, checkBox小天医.Checked.ToString());
        }


        private void textBox内容_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox内容.SelectAll();
        }

        private void textBox内容_VisibleChanged(object sender, EventArgs e)
        {
            button文本框.Enabled = !textBox内容.Visible;
            button导出.Enabled = !textBox内容.Visible;
        }

        private void button文本框_Click(object sender, EventArgs e)
        {
            textBox内容.Visible = true;
        }

    }
    //public class HttpWebResponseUtility
    //{
    //    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    //    /// <summary>  
    //    /// 创建GET方式的HTTP请求  
    //    /// </summary>  
    //    /// <param name="url">请求的URL</param>  
    //    /// <param name="timeout">请求的超时时间</param>  
    //    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    //    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    //    /// <returns></returns>  
    //    public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
    //    {
    //        if (string.IsNullOrEmpty(url))
    //        {
    //            throw new ArgumentNullException("url");
    //        }
    //        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
    //        request.Method = "GET";
    //        request.UserAgent = DefaultUserAgent;
    //        if (!string.IsNullOrEmpty(userAgent))
    //        {
    //            request.UserAgent = userAgent;
    //        }
    //        if (timeout.HasValue)
    //        {
    //            request.Timeout = timeout.Value;
    //        }
    //        if (cookies != null)
    //        {
    //            request.CookieContainer = new CookieContainer();
    //            request.CookieContainer.Add(cookies);
    //        }
    //        return request.GetResponse() as HttpWebResponse;
    //    }
    //    /// <summary>  
    //    /// 创建POST方式的HTTP请求  
    //    /// </summary>  
    //    /// <param name="url">请求的URL</param>  
    //    /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
    //    /// <param name="timeout">请求的超时时间</param>  
    //    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    //    /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
    //    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    //    /// <returns></returns>  
    //    public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
    //    {
    //        if (string.IsNullOrEmpty(url))
    //        {
    //            throw new ArgumentNullException("url");
    //        }
    //        if (requestEncoding == null)
    //        {
    //            throw new ArgumentNullException("requestEncoding");
    //        }
    //        HttpWebRequest request = null;
    //        //如果是发送HTTPS请求  
    //        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
    //        {
    //            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
    //            request = WebRequest.Create(url) as HttpWebRequest;
    //            request.ProtocolVersion = HttpVersion.Version10;
    //        }
    //        else
    //        {
    //            request = WebRequest.Create(url) as HttpWebRequest;
    //        }
    //        request.Method = "POST";
    //        request.ContentType = "application/x-www-form-urlencoded";

    //        if (!string.IsNullOrEmpty(userAgent))
    //        {
    //            request.UserAgent = userAgent;
    //        }
    //        else
    //        {
    //            request.UserAgent = DefaultUserAgent;
    //        }

    //        if (timeout.HasValue)
    //        {
    //            request.Timeout = timeout.Value;
    //        }
    //        if (cookies != null)
    //        {
    //            request.CookieContainer = new CookieContainer();
    //            request.CookieContainer.Add(cookies);
    //        }
    //        //如果需要POST数据  
    //        if (!(parameters == null || parameters.Count == 0))
    //        {
    //            StringBuilder buffer = new StringBuilder();
    //            int i = 0;
    //            foreach (string key in parameters.Keys)
    //            {
    //                if (i > 0)
    //                {
    //                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
    //                }
    //                else
    //                {
    //                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
    //                }
    //                i++;
    //            }
    //            byte[] data = requestEncoding.GetBytes(buffer.ToString());
    //            using (Stream stream = request.GetRequestStream())
    //            {
    //                stream.Write(data, 0, data.Length);
    //            }
    //        }
    //        return request.GetResponse() as HttpWebResponse;
    //    }

    //    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    //    {
    //        return true; //总是接受  
    //    }
    //}

}
