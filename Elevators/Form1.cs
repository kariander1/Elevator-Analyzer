using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Elevators
{
    public partial class Form1 : Form
    {
        frmNew newStructure;

        Building b;
     

        public Form1()
        {
            InitializeComponent();
        }

     
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newStructure=new frmNew();
            // Do Dialog thing
            if (newStructure.ShowDialog() == DialogResult.OK)
            {
                if (b != null)
                    b.Dispose();
                b = new Building(frmNew.flr,frmNew.ele);
                Controls.Add(b);
                tlCoordinates.Text = this.Size.ToString();
           
            }
           
            
        }

        

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //tlCoordinates.Text = e.Location.ToString();
            tlCoordinates.Text = PointToClient(Cursor.Position).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            b.GetElevators()[0].SetDestFloor(b.GetFloors()[int.Parse(textBox1.Text)]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            b.GetElevators()[1].SetDestFloor(b.GetFloors()[int.Parse(textBox1.Text)]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            b.GeneratePerson();
        }

        private void addElevatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            b.AddElevator();
            b.DrawElevators();
            
        }

        private void addFloorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            b.AddFloor();
            b.DrawFloors();
            
        }
    }
}
