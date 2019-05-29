using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Entity;

namespace SELENIUMitog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ChromeDriver ChromeDriver = new ChromeDriver();
            ChromeDriver.Navigate().GoToUrl("https://vk.com/feed");
            Login(ChromeDriver, "email", "79581926046");
            Login(ChromeDriver, "pass", "Password51ts");
            List<IWebElement> webElements = ChromeDriver.FindElementsById("login_button").ToList();
            foreach (IWebElement item in webElements)
            {
                if (!item.Displayed)
                {
                    continue;
                }
                if (!item.Text.ToLower().Equals("войти"))
                {
                    continue;
                }
                item.Click();
                break;
            }
            Thread.Sleep(5000);
            Thread OI = new Thread(WriteId);
            Thread OT = new Thread(WriteText);
            Thread OIm = new Thread(WriteImg);
            Thread All = new Thread(ReadAll);
            
            OI.Start();
            OT.Start();
            OIm.Start();
            All.Start();

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(5000);
                Parser(ChromeDriver);
            }

            OI.Abort();
            OT.Abort();
            OIm.Abort();
            All.Abort();

            ChromeDriver.Quit();
        }

        private void Login(ChromeDriver chromeDriver, string name, string value)
        {
            List<IWebElement> webElements = (from item in chromeDriver.FindElementsByName(name) where item.Displayed select item).ToList();
            if (!webElements.Any())
                return;
            webElements[0].SendKeys(value);
        }

        string textJson, imgJson, linkJson;
        Dictionary<string, string> textDic = new Dictionary<string, string>();
        Dictionary<string, string> linkDic = new Dictionary<string, string>();
        Dictionary<string, string[]> imgDic = new Dictionary<string, string[]>();

        bool FileR = false;

        private void Parser(ChromeDriver chromeDriver)
        {
            List<IWebElement> webElements = (from item in chromeDriver.FindElements(By.CssSelector("[id^=post-]")) select item).ToList();
            foreach (IWebElement item in webElements)
            {
                string id, text, link, img;
                id = item.GetAttribute("id").ToString();
                if (linkDic.ContainsKey(id)/* && System.IO.File.Exists("Links.json")*/)
                {
                    continue;
                }
                if (item.FindElements(By.CssSelector("._post_content .post_link")).Count != 0)
                {
                    link = item.FindElement(By.CssSelector("._post_content .post_link")).GetAttribute("href").ToString();
                    linkDic.Add(id, link);
                }
                if (item.FindElements(By.CssSelector("._post_content .wall_post_text")).Count != 0)
                {
                    text = item.FindElement(By.CssSelector("._post_content .wall_post_text")).Text;
                    textDic.Add(id, text);
                }
                if (item.FindElements(By.CssSelector(".wall_post_cont >.page_post_sized_thumbs > a[onclick *= showPhoto]")).Count != 0)
                {
                    List<IWebElement> imgList = (from itemImg in item.FindElements(By.CssSelector(".wall_post_cont >.page_post_sized_thumbs > a[onclick *= showPhoto]"))
                                                 select itemImg).ToList();
                    int j = 0;
                    string[] imgArr = new string[imgList.Count];
                    foreach (IWebElement itemImg in imgList)
                    {
                        img = itemImg.GetAttribute("style").ToString();
                        img = img.Substring(img.IndexOf('/') - 6);
                        img = img.Substring(0, img.Length - 3);
                        imgArr[j] = img;
                        j++;
                    }
                    imgDic.Add(id, imgArr);
                }
            }

            if (!System.IO.File.Exists("Links.json"))
            {

                linkJson = JsonConvert.SerializeObject(linkDic);
                textJson = JsonConvert.SerializeObject(textDic);
                imgJson = JsonConvert.SerializeObject(imgDic);
            }
            


            //if (!backgroundWorker1.WorkerReportsProgress && !backgroundWorker2.IsBusy && !backgroundWorker3.IsBusy && !backgroundWorker4.IsBusy)
            //{
            //    backgroundWorker1.RunWorkerAsync();
            //    backgroundWorker2.RunWorkerAsync();
            //    backgroundWorker3.RunWorkerAsync();
            //    backgroundWorker4.RunWorkerAsync();

            //}
            //else
            //{
            //    backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            //    backgroundWorker2.DoWork += new DoWorkEventHandler(BackgroundWorker2_DoWork);
            //    backgroundWorker3.DoWork += new DoWorkEventHandler(BackgroundWorker3_DoWork);
            //    //backgroundWorker4.DoWork += new DoWorkEventHandler(BackgroundWorker4_DoWork);
            //}
            for (int i = 0; i < 10; i++)
                chromeDriver.ExecuteScript("window.scrollBy(0,1920)");
        }

        private void WriteId()
        {
            for (; ; )
            {
                while (FileR == true)
                    Thread.Sleep(50);
                FileR = true;

                if (!System.IO.File.Exists("Links.json"))
                {
                    linkJson = JsonConvert.SerializeObject(linkDic);
                    File.WriteAllText("Links.json", linkJson);
                }
                else
                {
                    Dictionary<string, string> links = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Links.json"));
                    Dictionary<string, string> linkJsonDic = linkDic.Union(links).ToDictionary(x => x.Key, x => x.Value);
                    linkJson = JsonConvert.SerializeObject(linkJsonDic);
                    File.WriteAllText("Links.json", linkJson);
                }
                FileR = false;
                Thread.Sleep(300);
            }

        }
        private void WriteText()
        {
            for (; ; )
            {
                while (FileR == true)
                    Thread.Sleep(50);
                FileR = true;
                if (!System.IO.File.Exists("Text.json"))
                {
                    textJson = JsonConvert.SerializeObject(textDic);
                    File.WriteAllText("Text.json", textJson);
                }
                else
                {
                    Dictionary<string, string> text = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Text.json"));
                    Dictionary<string, string> textJsonDic = textDic.Union(text).ToDictionary(x => x.Key, x => x.Value);
                    textJson = JsonConvert.SerializeObject(textJsonDic);
                    File.WriteAllText("Text.json", textJson);

                }
                FileR = false;
                Thread.Sleep(300);
            }

        }

        private void WriteImg()
        {
            for (; ; )
            {
                while (FileR == true)
                    Thread.Sleep(50);
                FileR = true;
                if (!System.IO.File.Exists("Img.json"))
                {
                    imgJson = JsonConvert.SerializeObject(imgDic);
                    File.WriteAllText("Img.json", imgJson);
                }
                else
                {
                    Dictionary<string, string[]> img = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText("Img.json"));
                    Dictionary<string, string[]> imgJsonDic = new Dictionary<string, string[]>();
                    foreach (string s in img.Keys)
                    {
                        imgJsonDic.Add(s, img[s]);
                    }

                    imgJson = JsonConvert.SerializeObject(imgJsonDic);
                    File.WriteAllText("Img.json", imgJson);

                }
                FileR = false;
                Thread.Sleep(300);
            }


        }

        private void ReadAll()
        {
            for (; ; )
            {
                while (FileR == true)
                    Thread.Sleep(50);
                FileR = true;
                ReadFile();
                FileR = false;
                Thread.Sleep(100);
            }
        }

        private void ReadFile()
        {
            Thread.Sleep(50);
            if (System.IO.File.Exists("Links.json") && System.IO.File.Exists("Text.json") && System.IO.File.Exists("Img.json"))
            {
                StreamReader id = new StreamReader("Links.json");
                StreamReader text = new StreamReader("Text.json");
                StreamReader img = new StreamReader("Img.json");
                Dictionary<string, string> Links = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Links.json"));
                Dictionary<string, string> Text = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Text.json"));
                Dictionary<string, string[]> Img = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText("Img.json"));

                int i = 0;

                string[] LINK = new string[Links.Values.Count];
                string[,] TEXT = new string[2, Links.Keys.Count];
                string[,] IMG = new string[2, Links.Keys.Count];
                foreach (string c in Links.Values)
                {
                    LINK[i] = c;
                    i++;
                }


                i = 0;
                foreach (string c in Links.Keys)
                {
                    TEXT[0, i] = c;
                    if (Text.ContainsKey(TEXT[0, i]))
                    {
                        TEXT[1, i] = Text[TEXT[0, i]];
                    }
                    else
                    {
                        TEXT[1, i] = null;
                    }
                    i++;
                }


                string value;
                i = 0;
                foreach (string c in Links.Keys)
                {
                    value = null;
                    IMG[0, i] = c;
                    if (Img.ContainsKey(IMG[0, i]))
                    {
                        for (int j = 0; j < Img[TEXT[0, i]].GetLength(0); j++)
                            value = value + Img[IMG[0, i]][j] + " ; ";

                        IMG[1, i] = value;
                    }
                    i++;
                }
                AddToDb(Links, LINK, TEXT, IMG);
                Thread.Sleep(200);
                id.Close();
                text.Close();
                img.Close();
            }
        }

        private void AddToDb(Dictionary<string, string> Links, string[] LINK, string[,] TEXT, string[,] IMG)
        {
            using (UserContext db = new UserContext())
            {
                int i = 0, j = 0;

                foreach (string c in Links.Keys)
                {
                    Post pt = null;
                    pt = db.Posts.Find(c);
                    if (pt == null)
                    {
                        Post post = new Post { Id = c, Link = LINK[i], Text = TEXT[1, i], Img = IMG[1, i] };
                        db.Posts.Add(post);
                        db.SaveChanges();
                    }
                    i++;
                }
            }
        }

    }

    class UserContext : DbContext
    {
        public UserContext()
            : base("DbConnection")
        { }

        public DbSet<Post> Posts { get; set; }
    }
    public class Post
    {
        public string Id { get; set; }
        public string Link { get; set; }
        public string Text { get; set; }
        public string Img { get; set; }
    }

}

