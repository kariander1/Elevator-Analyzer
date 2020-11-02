using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
namespace Elevators
{
    class Elevator : Panel
    {
        BackgroundWorker worker;
        BackgroundWorker idleWorker;
        private PictureBox arrow;

        private bool active = false,ascending=false;
        private Stopwatch totalTime;
        private Stopwatch workTime;
        public static object elLock = new object();
        private int pixelTravel = 0;
        private int minFloor;
        private int maxFloor;
        private Floor destFloor;
        private int capacity;
        private int onBoard;
        private Label axis;
        private double velocity=100;// Pixels per second
        private Floor currentFloor;
      
        private List<Person>[] people;
        public Elevator(int minF,int maxF,Floor current)
        {
            this.onBoard = 0;
            people = new List<Person>[Building.floors.Count];
            for (int i = 0; i < people.Length; i++)
            {
                people[i] = new List<Person>();
            }
            #region Arrow
            arrow = new PictureBox();
            arrow.Size = new Size(8, 8);
            arrow.SizeMode = PictureBoxSizeMode.StretchImage;
            arrow.Location = new Point(1, 1);
            Controls.Add(arrow);
            arrow.Click += Elevator_Click;
            #endregion
            this.Click += Elevator_Click;
            totalTime = new Stopwatch();
            totalTime.Start();
            workTime = new Stopwatch();
  
            this.destFloor = currentFloor;

            idleWorker = new BackgroundWorker();
            idleWorker.DoWork += idleWorker_DoWork;

            worker = new BackgroundWorker();
            worker.DoWork+=worker_DoWork;
            this.minFloor = minF;
            this.maxFloor = maxF;
            currentFloor = current;
            Random rnd=new Random();
            //this.Location = new Point(
            this.Size = new Size(10, 10);
            this.BackColor = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            this.capacity = 10;
            this.axis = new Label();
            axis.BackColor = Color.Firebrick;

            idleWorker.RunWorkerAsync();
        }

        void idleWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (worker.IsBusy)
            {
            }
       
            Floor down = currentFloor;
            Floor up = currentFloor;
            while (true)
            {
                lock (elLock)
                {
                    if (Building.requests.Count != 0 && Building.requests[0] != null)
                    {
                   
                        SetDestFloor(Building.requests[0].GetSource());
                  
                        break;
                    }
                }
                //lock(this)
                //{
                //if (up.Waiting()&&!up.beingHandeled)
                //{
                //    up.beingHandeled = true;
                //    SetDestFloor(up);
                //    break;
                //}
                //else
                //{
                //    if (up.GetNext() == null)
                //        up = currentFloor;
                //    else
                //        up = up.GetNext();                    
                //}
                //}
                //lock (this)
                //{
                //    if (down.Waiting()&&!down.beingHandeled)
                //    {
                //        down.beingHandeled = true;
                //        SetDestFloor(down);
                //        break;
                //    }
                //    else
                //    {
                //        if (down.GetPrev() == null)
                //            down = currentFloor;
                //        else
                //            down = down.GetPrev();
                //    }
                //}
            }
        }

        void Elevator_Click(object sender, EventArgs e)
        {
           
           
            MessageBox.Show(@"Total time: " + totalTime.Elapsed.ToString() + 
                "\n\nWork time: " + workTime.Elapsed.ToString() + 
                "\n\nEfficiency: " +  (((double)workTime.ElapsedTicks / (double)totalTime.ElapsedTicks)* 100).ToString().Substring(0,5) + "%"+
                "\n\nCurrent Floor: "+currentFloor.ToString());
        }
        public Label GetAxis()
        {
            return axis;
        }
        public void SetAxisSize(Size s)
        {
            this.axis.Size = s;
        }
        public void SetAxisPosition(Point p)
        {
            this.axis.Location = p;
        }
        public void ChangeFloor(Floor destFloor)
        {

            this.destFloor = destFloor;
                
            worker.RunWorkerAsync();
          

        }
        public bool isFull()
        {
            return onBoard == capacity;
        }
        public 
        void worker_DoWork(object sender,DoWorkEventArgs e)
        {
            while (idleWorker.IsBusy)
            {
            }
       
            active = true;
            workTime.Start();
            int modifier = 1;
            arrow.Image = Image.FromFile(Path.GetFullPath(@"..\..\Images\downArrow.png"));
            
            if (destFloor.GetIndex() > currentFloor.GetIndex())
            {
                modifier = -1;
                arrow.Image = Image.FromFile(Path.GetFullPath(@"..\..\Images\upArrow.png"));
            }
            
            bool flag = false;
            int sum = 0;
            if (destFloor.GetIndex() >= minFloor && destFloor.GetIndex() <= maxFloor)
            {
                while (!flag)
                {
                    

                    if (this.InvokeRequired&&!flag)
                    {
                        this.Invoke(new MethodInvoker(delegate { Location = new Point(Location.X, Location.Y + modifier); }));
                        sum++;
                    }

                    int interval = int.Parse(Math.Round(decimal.Parse((1000 / velocity).ToString())).ToString());
                    Thread.Sleep(interval);

                        if (sum % Building.floorspacing == 0)
                        {
                            if (modifier == 1)
                            {
                                
                                    currentFloor = currentFloor.GetPrev();
                                  
                                    HandleDescending();

                            }
                            else
                            {
                               
                                    currentFloor = currentFloor.GetNext();
                                    
                                    HandleAscending();                               

                            }
                            for (int i = 0; i < people[currentFloor.GetIndex()].Count; i++)// People taking off
                            {
                                Random rnd=new Random();
                              
                                people[currentFloor.GetIndex()][i].Drop(currentFloor);
                              
                                people[currentFloor.GetIndex()].Remove(people[currentFloor.GetIndex()][i]);// removes from elevator
                                onBoard--;
                                Thread.Sleep(10);
                            }
                        }
                        currentFloor.beingHandeled = false;
                        if (sum == pixelTravel)
                        {
                            Floor temp = CheckForFloors();
                            if (temp == null)
                                flag = true;
                            else
                                UpdatePixelTravel(temp);
                        }
                        Application.DoEvents();
            

                }
               
            }
            workTime.Stop();
            active = false;
            arrow.Image = null;
            idleWorker.RunWorkerAsync();
        }
        private Floor CheckForFloors()
        {
            Floor temp = currentFloor;
            if (ascending)
            {
                
                while (temp != null)
                {
                    if (temp.GetAscending().Count != 0)
                        return temp;
                    temp = temp.GetNext();
                }
            }
            else
            {
                while (temp != null)
                {
                    if (temp.GetDescending().Count != 0)
                        return temp;
                    temp = temp.GetPrev();
                }
            }
            return null;
        }
        private bool HandleAscending()
        {
            bool flag = false;
            if (!isFull())
            {
                List<Person> temp = new List<Person>();
                foreach (Person p in currentFloor.GetAscending())
                {
                    if (p.InvokeRequired)
                    {
                        p.Invoke(new MethodInvoker(delegate
                        {
                            p.Visible = false;
                        }));
                    }
                    flag = true;
                    Building.requests.Remove(p.GetRequest());
                    people[p.GetRequest().GetDestination().GetIndex()].Add(p);
                    temp.Add(p);

                    onBoard++;
                    if (isFull())
                        break;
                    HandleRequest(p.GetRequest());
                }
                foreach (Person p in temp)
                {
                    currentFloor.GetAscending().Remove(p);
                }
            }
            return flag;
        }
        public void HandleRequest(Request req)
        {
           
            if (active)
            {
                if ((req.GetDestination().GetIndex() > this.destFloor.GetIndex() == ascending))
                    SetDestFloor(req.GetDestination());
            }
            else
            {
                SetDestFloor(req.GetDestination());
            }

                
        }
        public Floor GetCurrentFloor()
        {
            return this.currentFloor;
        }


        internal double GetVelocity()
        {
            return this.velocity;
        }

        internal void SetMaxFloor(int p)
        {
            this.maxFloor = p;
        }
        public void UpdatePixelTravel(Floor f)
        {
            if (active)
            {
                if ((WillAscend(f) && ascending) || (!WillAscend(f) && !ascending))
                {
                    pixelTravel += Math.Abs((f.Location.Y - destFloor.Location.Y));
                    this.destFloor = f;
                }
            }
            else
            {
                if (WillAscend(f))
                {
                    pixelTravel = Math.Abs(f.Location.Y - this.Location.Y) + 10;
                    ascending = true;
                }
                else
                {
                    pixelTravel = Math.Abs(f.Location.Y - this.Location.Y) - 10;
                    ascending = false;
                }
                ChangeFloor(f);
            }
        }
        public void SetDestFloor(Floor f)
        {

            if (f == currentFloor)
            {
                if (!HandleDescending())
                    HandleAscending();
            }

            else
            {
                UpdatePixelTravel(f);
            }
            //pixelTravel = Math.Abs(Building.floorspacing * (f.GetIndex() - this.currentFloor.GetIndex()));
          
            
           
                
        }

        private bool HandleDescending()
        {
            bool flag = false;
            if (!isFull())
            {
                List<Person> temp = new List<Person>();
                foreach (Person p in currentFloor.GetDescending())
                {
                    if (p.InvokeRequired)
                    {
                        p.Invoke(new MethodInvoker(delegate
                        {
                            p.Visible = false;
                        }));
                    }
                    temp.Add(p);
                    Building.requests.Remove(p.GetRequest());
                    flag = true;
                    people[p.GetRequest().GetDestination().GetIndex()].Add(p);
                    
                    onBoard++;
                    if (isFull())
                        break;
                    HandleRequest(p.GetRequest());
                }
                foreach (Person p in temp)
                {
                    currentFloor.GetDescending().Remove(p);
                }
            }
            return flag;
        }
        public bool WillAscend(Floor dest)
        {
            return (dest.GetIndex() - currentFloor.GetIndex())>0;
        }
    }
}
