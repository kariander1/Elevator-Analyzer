using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
namespace Elevators
{
    class Person : PictureBox
    {
        private List<Floor> visitedFloors;
        private Request request;
        private Floor CurrentFloor;
        private BackgroundWorker worker;
        private BackgroundWorker wait;
        public Person(Floor floor)
        {
            
            wait = new BackgroundWorker();
            wait.WorkerSupportsCancellation = true;
            wait.DoWork += wait_DoWork;
            visitedFloors = new List<Floor>();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            this.CurrentFloor=floor;
            visitedFloors.Add(floor);
            this.Size = new Size(7, 14);
            Random rnd = new Random();
            if (rnd.Next(0, 2) == 1)
                this.Image = Image.FromFile(Path.GetFullPath(@"..\..\Images\man-figure-symbol-hi.png"));
            else
                this.Image = Image.FromFile(Path.GetFullPath(@"..\..\Images\woman.png"));
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }

        void wait_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(10000);
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.BackColor = Color.Red;
                }));
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //wait.CancelAsync();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.BackColor = Color.Transparent;
                }));
            }
            Random rnd = new Random();

            Thread.Sleep(rnd.Next(5000, 10000));
            GenerateRequest();
        }

      
        public void GenerateRequest()
        {
            int x=0;
            Random rnd=new Random();
            do
            {
                x=rnd.Next(0,Building.floors.Count);
            }while(x==CurrentFloor.GetIndex());
            request = new Request(CurrentFloor, Building.floors[x]);
        
            CurrentFloor.HandlePerson(this);
            //wait.RunWorkerAsync();
        }
        public Request GetRequest()
        {
            return this.request;
        }
        public void ResetRequest()
        {
            this.request = null;
        }
        public void Drop(Floor f)
        {
            visitedFloors.Add(f);
            Random rnd = new Random();
            this.CurrentFloor = f;
            ResetRequest();
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.Visible = true;

                    this.Location = new Point(rnd.Next(0, CurrentFloor.Width - 7), CurrentFloor.Location.Y - 14);
                }));
            }
            this.CurrentFloor.AddPerson(this);
            while (worker.IsBusy)
            {

            }
            worker.RunWorkerAsync();
        }
    }
}
