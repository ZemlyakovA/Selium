using System.Collections.Generic;
using System.ServiceProcess;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity;
using System.Threading;
using System.Data;
using System.Linq;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread parserThread = new Thread(new ThreadStart(AddBd));
            parserThread.Start();
        }

        private void AddBd()
        {
            for (; ; )
            {
                Dictionary<string, string> Links = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"C:\Users\User\Desktop\БИСО-01-17\сенениум\SELENIUMinog\SELENIUMinog\bin\Debug\Links.json"));
                Dictionary<string, string> Text = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"C:\Users\User\Desktop\БИСО-01-17\сенениум\SELENIUMinog\SELENIUMinog\bin\Debug\Text.json"));
                Dictionary<string, string[]> Img = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@"C:\Users\User\Desktop\БИСО-01-17\сенениум\SELENIUMinog\SELENIUMinog\bin\Debug\Img.json"));

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

                i = 0;
                Models.MyDataBaseEntities db = new Models.MyDataBaseEntities();
                List<Models.Table> lst2 = (from t in db.Tables select t).ToList();
                foreach (string c in Links.Keys)
                {


                    Models.Table t1 = new Models.Table()
                    {
                        Id = c,
                        Link = LINK[i],
                        Text = TEXT[1, i],
                        Img = IMG[1,i]
                    };
                    db.Tables.Add(t1);
                    db.SaveChanges();
                    i++;
                }
                
                File.WriteAllText(@"C:\Users\User\Desktop\Новая папка (2)\qwe.txt", "123123123123");
                


            }
        }

        protected override void OnStop()
        {
            
        }
    }
}
