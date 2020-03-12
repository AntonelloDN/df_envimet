using Grasshopper.Kernel;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using System.Drawing;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.UI_GH
{
    public abstract class ComponentWithDirectionLogic : GH_Component
    {

        public FaceDirection Direction { get; set; }

        public ComponentWithDirectionLogic(string name, string nickName, string description, string category, string subCategory)
            : base(name, nickName, description, category, subCategory)
        {
            Direction = FaceDirection.X;
        }

        public override void CreateAttributes()
        {
            m_attributes = new CustomAttributes(this);
        }

    }

    public class CustomAttributes : GH_ComponentAttributes
    {
        public CustomAttributes(ComponentWithDirectionLogic owner) : base(owner) { }

        #region Custom layout logic
        private RectangleF DirectionX { get; set; }
        private RectangleF DirectionY { get; set; }
        private RectangleF DirectionZ { get; set; }

        protected override void Layout()
        {
            base.Layout();
            DirectionX = new RectangleF(Bounds.X, Bounds.Bottom, Bounds.Width, 20);
            DirectionY = new RectangleF(Bounds.X, Bounds.Bottom + 20, Bounds.Width, 20);
            DirectionZ = new RectangleF(Bounds.X, Bounds.Bottom + 40, Bounds.Width, 20);
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + 60);
        }
        #endregion

        #region Custom Mouse handling
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ComponentWithDirectionLogic comp = Owner as ComponentWithDirectionLogic;

                if (DirectionX.Contains(e.CanvasLocation))
                {
                    if (comp.Direction == FaceDirection.X) return GH_ObjectResponse.Handled;
                    comp.RecordUndoEvent("Direction X");
                    comp.Direction = FaceDirection.X;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (DirectionY.Contains(e.CanvasLocation))
                {
                    if (comp.Direction == FaceDirection.Y) return GH_ObjectResponse.Handled;
                    comp.RecordUndoEvent("Direction Y");
                    comp.Direction = FaceDirection.Y;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (DirectionZ.Contains(e.CanvasLocation))
                {
                    if (comp.Direction == FaceDirection.Z) return GH_ObjectResponse.Handled;
                    comp.RecordUndoEvent("Direction Z");
                    comp.Direction = FaceDirection.Z;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
        #endregion

        #region Custom Render logic
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.Objects:
                    base.RenderComponentCapsule(canvas, graphics, true, false, false, true, true, true);

                    ComponentWithDirectionLogic comp = Owner as ComponentWithDirectionLogic;

                    GH_Capsule btnX = GH_Capsule.CreateCapsule(DirectionX, comp.Direction == FaceDirection.X ? GH_Palette.Black : GH_Palette.White);
                    btnX.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                    btnX.Dispose();

                    GH_Capsule btnY = GH_Capsule.CreateCapsule(DirectionY, comp.Direction == FaceDirection.Y ? GH_Palette.Black : GH_Palette.White);
                    btnY.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                    btnY.Dispose();

                    GH_Capsule btnZ = GH_Capsule.CreateCapsule(DirectionZ, comp.Direction == FaceDirection.Z ? GH_Palette.Black : GH_Palette.White);
                    btnZ.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                    btnZ.Dispose();

                    graphics.DrawString("X", GH_FontServer.Standard, comp.Direction == FaceDirection.X ? Brushes.Gold : Brushes.Black, DirectionX, GH_TextRenderingConstants.CenterCenter);
                    graphics.DrawString("Y", GH_FontServer.Standard, comp.Direction == FaceDirection.Y ? Brushes.Gold : Brushes.Black, DirectionY, GH_TextRenderingConstants.CenterCenter);
                    graphics.DrawString("Z", GH_FontServer.Standard, comp.Direction == FaceDirection.Z ? Brushes.Gold : Brushes.Black, DirectionZ, GH_TextRenderingConstants.CenterCenter);

                    break;

                default:
                    base.Render(canvas, graphics, channel);
                    break;
            }
        }
        #endregion
    }
}
