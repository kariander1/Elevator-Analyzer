using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Elevators
{
    class Building : Panel
    {
        public static int floorspacing = 0;
        private int numFloors=0;
        private List<Elevator> elevators;
        public static List<Floor> floors;
        public static List<Request> requests;
        bool adjust=false;
      
        public Building(int fls,int ele)
        {
            #region FourArrow
            PictureBox move = new PictureBox();
            move.Size = new Size(14, 14);
            move.Image=Image.FromFile(Path.GetFullPath(@"..\..\Images\hhh.png"));
            move.SizeMode = PictureBoxSizeMode.StretchImage;
            move.MouseDown += b_MouseDown;
            move.MouseMove += b_MouseMove;
            move.MouseUp += b_MouseUp;
            Controls.Add(move);

            #endregion

            requests = new List<Request>();
            elevators = new List<Elevator>();
            floors = new List<Floor>();
            this.numFloors = fls;
            this.BackColor=Color.Gray;
            this.BorderStyle = BorderStyle.FixedSingle;            
            this.Location = new Point(30, 55);
            if (ele * 80 > Form1.ActiveForm.Size.Width - 60)
                this.Size = new Size(ele * 80, 20 * fls);
            else
                this.Size = new Size(Form1.ActiveForm.Size.Width-60 ,20*fls);

            floorspacing = Height / (fls + 1);
           
         

            int spacing = Height / (fls + 1);
            #region Floor
            for (int i = 0; i < fls+1; i++)
            {
                AddFloor();
                
                
                

                
            }
            DrawFloors();
            #endregion

            #region Elevator
            for (int i = 0; i < ele; i++)
            {

                AddElevator();

            }
            DrawElevators();
            #endregion
        }
        void b_MouseDown(object sender, MouseEventArgs e)
        {
            adjust = true;

        }

        void b_MouseUp(object sender, MouseEventArgs e)
        {
            adjust = false;
        }

        void b_MouseMove(object sender, MouseEventArgs e)
        {
            if (adjust)
            {
                Location = Point.Subtract(FindForm().PointToClient(Cursor.Position), new Size(7, 7));
            }
        }
        public void AddFloor()
        {
            Floor temp;
            if (floors.Count == 0)
                temp = new Floor(floors.Count, null);
            else
            {
                temp = floors[0];
                while (temp.GetNext() != null)
                {
                    temp = temp.GetNext();
                }
                temp.SetNext(new Floor(floors.Count , temp));
                temp = temp.GetNext();
            }
         
            temp.Size = new Size(this.Size.Width, 1);
            floors.Add(temp);
            Controls.Add(temp);
            foreach (Elevator e in elevators)
            {
                e.SetMaxFloor(floors.Count);
            }
        }
        public void DrawFloors()
        {
            for (int i = 0; i < floors.Count; i++)
            {
                floors[i].Location = new Point(0, Height - floorspacing * (i));
                Label nm=new Label();
                nm.Width = 20;
                if (i == 0)
                {
                    nm.Text = "G";
                    nm.Location = new Point(0, Height - floorspacing * (i) - 10);
                }
                else
                {                
                    nm.Text = (i).ToString();
                    nm.Location = new Point(0, Height - floorspacing * (i) - 7);
                }
                Controls.Add(nm);
            }
        }
        public void AddElevator()
        {
            Elevator temp = new Elevator(0, floors.Count,floors[0]);      
         
            temp.SetAxisSize(new Size(2, this.Size.Height));
          
            elevators.Add(temp);

           
        }
        public void DrawElevators()
        {
            int spacing = this.Size.Width / (elevators.Count + 1);
            for (int i = 0; i < elevators.Count; i++)
            {
                elevators[i].Location = new Point(spacing * (i + 1), this.Size.Height - 10 -(Building.floorspacing*elevators[i].GetCurrentFloor().GetIndex()));
                elevators[i].SetAxisPosition(new Point(spacing * (i + 1) + 4, 0));
                Controls.Add(elevators[i]);
                Controls.Add(elevators[i].GetAxis());
            }
        }
        public List<Elevator> GetElevators()
        {
            return (elevators);
        }
        public void GeneratePerson()
        {
            Person p = new Person(floors[0]);
            p.Location = new Point(elevators[0].Location.X + 11, floors[0].Location.Y - p.Size.Height);
            floors[0].AddPerson(p);
            Controls.Add(p);
            p.GenerateRequest();
        }

        internal List<Floor> GetFloors()
        {
            return floors;
        }
    }
}
