//
// ToolStripDropDown.cs
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Jonathan Pobst
//
// Authors:
//	Jonathan Pobst (monkey@jpobst.com)
//

#if NET_2_0
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace System.Windows.Forms
{
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible (true)]
	public class ToolStripDropDown : ToolStrip
	{
		private bool allow_transparency;
		private bool auto_close;
		private bool drop_shadow_enabled = true;
		private double opacity = 1D;
		private ToolStripItem owner_item;

		#region Public Constructor
		public ToolStripDropDown () : base ()
		{
			SetStyle (ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.ResizeRedraw, true);

			this.auto_close = true;
			is_visible = false;
			this.GripStyle = ToolStripGripStyle.Hidden;
		}
		#endregion

		#region Public Properties
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool AllowTransparency {
			get { return allow_transparency; }
			set {
				if (value == allow_transparency)
					return;

				if ((XplatUI.SupportsTransparency () & TransparencySupport.Set) != 0) {
					allow_transparency = value;

					if (value) 
						XplatUI.SetWindowTransparency (Handle, Opacity, Color.Empty);
					else
						UpdateStyles (); // Remove the WS_EX_LAYERED style
				}
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { base.Anchor = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool AutoClose
		{
			get { return this.auto_close; }
			set { this.auto_close = value; }
		}

		[DefaultValue (true)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public ContextMenu ContextMenu {
			get { return null; }
			set { }
		}

		//[Browsable (false)]
		//[EditorBrowsable (EditorBrowsableState.Never)]
		//public ContextMenuStrip ContextMenuStrip {
		//        get { return null; }
		//        set { }
		//}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = value; }
		}
		
		public bool DropShadowEnabled {
			get { return this.drop_shadow_enabled; }
			set {
				if (this.drop_shadow_enabled == value)
					return;
					
				this.drop_shadow_enabled = value;
				UpdateStyles ();	// Re-CreateParams
			}
		}

		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public ToolStripGripDisplayStyle GripDisplayStyle {
			get { return ToolStripGripDisplayStyle.Vertical; }
			set { }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public Padding GripMargin {
			get { return Padding.Empty; }
			set { }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public Rectangle GripRectangle {
			get { return Rectangle.Empty; }
			set { }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public ToolStripGripStyle GripStyle {
			get { return base.GripStyle; }
			set { base.GripStyle = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}

		public double Opacity {
			get { return this.opacity; }
			set {
				if (this.opacity == value)
					return;
					
				this.opacity = value;
				this.allow_transparency = true;

				UpdateStyles ();
				XplatUI.SetWindowTransparency (Handle, opacity, Color.Empty);
			}
		}

		public ToolStripItem OwnerItem {
			get { return this.owner_item; }
			set { this.owner_item = value; }
		}
		
		public Region Region {
			get { return base.Region; }
			set { base.Region = value; }
		}

		[Localizable (true)]
		public override RightToLeft RightToLeft {
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool Stretch {
			get { return false; }
			set { }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int TabIndex {
			get { return 0; }
			set { }
		}

		public bool TopLevel {
			get { return this.TopMost; }
			set { }
		}
		
		[Localizable (true)]
		public bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		#endregion

		#region Protected Properties
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;

				cp.Style = unchecked ((int)(WindowStyles.WS_POPUP | WindowStyles.WS_CLIPCHILDREN));
				cp.ClassStyle = unchecked ((int)0x82000000);
				cp.ExStyle |= (int)(WindowExStyles.WS_EX_TOOLWINDOW | WindowExStyles.WS_EX_TOPMOST);

				if (Opacity < 1.0 && allow_transparency)
					cp.ExStyle |= (int)WindowExStyles.WS_EX_LAYERED;

				return cp;
			}
		}

		protected override DockStyle DefaultDock {
			get { return DockStyle.None; }
		}

		protected override Padding DefaultPadding {
			get { return new Padding (1, 2, 1, 2); }
		}

		protected override bool DefaultShowItemToolTips {
			get { return true; }
		}

		//protected internal override Size MaxItemSize {
		//        get {  return new Size (Screen.PrimaryScreen.Bounds.Width - 2, Screen.PrimaryScreen.Bounds.Height - 34); }
		//}

		protected virtual bool TopMost {
			get { return true; }
		}
		#endregion

		#region Public Methods
		public void Close ()
		{
			this.Close (ToolStripDropDownCloseReason.CloseCalled);
		}

		public void Close (ToolStripDropDownCloseReason reason)
		{
			// Give users a chance to cancel the close
			ToolStripDropDownClosingEventArgs e = new ToolStripDropDownClosingEventArgs (reason);
			this.OnClosing (e);

			if (e.Cancel)
				return;

			// Don't actually close if AutoClose == true unless explicitly called
			if (!this.auto_close && reason != ToolStripDropDownCloseReason.CloseCalled)
				return;

			// Owner MenuItem needs to be told to redraw (it's no longer selected)
			if (owner_item != null)
				(owner_item as ToolStripMenuItem).Invalidate ();

			// Recursive hide all child dropdowns
			foreach (ToolStripItem tsi in this.Items)
				if (tsi is ToolStripMenuItem)
					(tsi as ToolStripMenuItem).HideDropDown (reason);

			// Hide this dropdown
			this.Hide ();
			
			this.OnClosed (new ToolStripDropDownClosedEventArgs (reason));
		}

		public void Show ()
		{
			CancelEventArgs e = new CancelEventArgs ();
			this.OnOpening (e);
			
			if (e.Cancel)
				return;
				
			base.Show ();
			
			this.OnOpened (EventArgs.Empty);
		}
		
		public void Show (Point screenLocation)
		{
			this.Location = screenLocation;
			Show ();
		}
		
		public void Show (Control control, Point position)
		{
			if (control == null)
				throw new ArgumentNullException ("control");
				
			this.Location = control.PointToScreen (position);
		}
		
		public void Show (int x, int y)
		{
			this.Location = new Point (x, y);
			Show ();
		}
		#endregion

		#region Protected Methods
		protected override void CreateHandle ()
		{
			base.CreateHandle ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		protected virtual void OnClosed (ToolStripDropDownClosedEventArgs e)
		{
			if (Closed != null) Closed (this, e);
		}

		protected virtual void OnClosing (ToolStripDropDownClosingEventArgs e)
		{
			if (Closing != null) Closing (this, e);
		}

		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		protected override void OnItemClicked (ToolStripItemClickedEventArgs e)
		{
			base.OnItemClicked (e);
		}

		protected override void OnLayout (LayoutEventArgs e)
		{
			base.OnLayout (e);

			// Find the widest menu item
			int widest = 0;

			foreach (ToolStripItem tsi in this.Items)
				if (tsi.GetPreferredSize (Size.Empty).Width > widest)
					widest = tsi.GetPreferredSize (Size.Empty).Width;

			int x = this.Padding.Left;
			widest += 68 - this.Padding.Horizontal;
			int y = this.Padding.Top;

			foreach (ToolStripItem tsi in this.Items) {
				y += tsi.Margin.Top;

				int height = 0;

				if (tsi is ToolStripSeparator)
					height = 7;
				else
					height = 22;

				tsi.SetBounds (new Rectangle (x, y, widest, height));
				y += tsi.Height + tsi.Margin.Bottom;
			}

			this.Size = new Size (widest + this.Padding.Horizontal, y + this.Padding.Bottom);// + 2);
		}

		protected override void OnMouseUp (MouseEventArgs mea)
		{
			base.OnMouseUp (mea);
		}

		protected virtual void OnOpened (EventArgs e)
		{
			if (Opened != null) Opened (this, e);
		}

		protected virtual void OnOpening (CancelEventArgs e)
		{
			if (Opening != null) Opening (this, e);
		}

		protected override void OnParentChanged (EventArgs e)
		{
			base.OnParentChanged (e);
		}

		protected override void OnVisibleChanged (EventArgs e)
		{
			base.OnVisibleChanged (e);
		}

		protected override bool ProcessDialogChar (char charCode)
		{
			return base.ProcessDialogChar (charCode);
		}

		protected override bool ProcessDialogKey (Keys keyData)
		{
			return base.ProcessDialogKey (keyData);
		}

		protected override bool ProcessMnemonic (char charCode)
		{
			return base.ProcessMnemonic (charCode);
		}

		protected override void ScaleCore (float dx, float dy)
		{
			base.ScaleCore (dx, dy);
		}

		protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore (x, y, width, height, specified);
		}

		protected override void SetVisibleCore (bool value)
		{
			// Copied from Control.cs, changed XPlatUI.SetVisible(,,) to NO_ACTIVATE
			if (value != is_visible) {
				if (value && (window.Handle == IntPtr.Zero) || !is_created)
					CreateControl ();

				is_visible = value;

				if (IsHandleCreated)
					XplatUI.SetVisible (Handle, value, false);

				OnVisibleChanged (EventArgs.Empty);

				if (value == false && parent != null && Focused) {
					Control container;

					// Need to start at parent, GetContainerControl might return ourselves if we're a container
					container = (Control)parent.GetContainerControl ();
					
					if (container != null)
						container.SelectNextControl (this, true, true, true, true);
				}

				if (parent != null)
					parent.PerformLayout (this, "visible");
				else
					PerformLayout (this, "visible");
			}
		}

		protected override void WndProc (ref Message m)
		{
			const int MA_NOACTIVATE = 0x0003;

			// Don't activate when the WM tells us to
			if ((Msg)m.Msg == Msg.WM_MOUSEACTIVATE) {
				m.Result = (IntPtr)MA_NOACTIVATE;
				return;
			}

			base.WndProc (ref m);
		}
		#endregion

		#region Public Events
		public event EventHandler BackgroundImageChanged;
		public event EventHandler BackgroundImageLayoutChanged;
		public event EventHandler BindingContextChanged;
		public event UICuesEventHandler ChangeUICues;
		public event ToolStripDropDownClosedEventHandler Closed;
		public event ToolStripDropDownClosingEventHandler Closing;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler ContextMenuChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler ContextMenuStripChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler DockChanged;
		public event EventHandler Enter;
		public event EventHandler FontChanged;
		public event EventHandler ForeColorChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event GiveFeedbackEventHandler GiveFeedback;
		public event HelpEventHandler HelpRequested;
		public event EventHandler ImeModeChanged;
		public event KeyEventHandler KeyDown;
		public event KeyPressEventHandler KeyPress;
		public event KeyEventHandler KeyUp;
		public event EventHandler Leave;
		public event EventHandler Opened;
		public event CancelEventHandler Opening;
		public event EventHandler RegionChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event ScrollEventHandler Scroll;
		public event EventHandler StyleChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler TabIndexChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler TabStopChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler TextChanged;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler Validated;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event CancelEventHandler Validating;
		#endregion
	}
}
#endif