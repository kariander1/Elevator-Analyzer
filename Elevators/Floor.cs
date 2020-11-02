using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Elevators
{
    class Floor : Label
    {
        private Floor nextFloor;
        private Floor prevFloor;
        private List<Person> onFloor;
        private List<Person> ascending;
        private List<Person> descending;
        public bool beingHandeled=false;
        private int index;
        public bool Waiting()
        {
            return (ascending.Count > 0 || descending.Count > 0);
        }
        public Floor(int nm,Floor prev)
        {
          
            onFloor = new List<Person>();
            ascending = new List<Person>();
            descending = new List<Person>();
            
            prevFloor = prev;
            nextFloor = null; ;
            this.index = nm;
            this.BackColor = Color.Black;
            
        }
        public void AddPerson(Person p)
        {
            onFloor.Add(p);
        }
        public List<Person> GetAscending()
        {
            return this.ascending;
        }
        public List<Person> GetDescending()
        {
            return this.descending;
        }
        public void HandlePerson(Person p)
        {
            Request temp = p.GetRequest();
            onFloor.Remove(p);
            Building.requests.Add(p.GetRequest());
            if (temp.IsAscending())           
                
                ascending.Add(p);
            
            else
                descending.Add(p);
            
        }
    
        public int GetIndex()
        {
            return this.index;
        }
        public override string ToString()
        {
            if (this.index == 0)
                return "Ground Floor";
            return "F : " + this.index;
        }
        public Floor GetPrev()
        {
            return prevFloor;
        }
        public Floor GetNext()
        {
            return nextFloor;
        }
        public void SetPrev(Floor prev)
        {
            this.prevFloor = prev;
        }
        public void SetNext(Floor next)
        {
            this.nextFloor = next;
        }
    }
}
