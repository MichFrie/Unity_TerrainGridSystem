using System;
using UnityEngine;
[Serializable]

    public abstract class CellBase : MonoBehaviour
    {
        // stuff missing

        public event EventHandler CellClicked;
        public event EventHandler CellHighlighted;
        public event EventHandler CellDehighlighted;
        
        protected virtual void OnMouseEnter()
        {
            if (CellHighlighted != null)
                CellHighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseExit()
        {
            if (CellDehighlighted != null)
                CellDehighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseDown()
        {
            if (CellClicked != null)
                CellClicked.Invoke(this, new EventArgs());
        }
    }
