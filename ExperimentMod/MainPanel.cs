using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using KianCommons.UI;

namespace ExperimentMod
{
    public class MainPanel : UIPanel
    {
        public static MainPanel Create() => UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;
        public static void Release() => DestroyImmediate(FindObjectOfType<MainPanel>()?.gameObject);

        UIDragHandle drag_;

        public override void Awake()
        {
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenHorizontally = autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 0, 3);
            this.backgroundSprite = "MenuPanel";


            drag_ = AddUIComponent<UIDragHandle>();
            drag_.height = 40;
            drag_.relativePosition = Vector3.zero;
            //drag_.target = parent;
            drag_.eventMouseUp += Drag__eventMouseUp;

            AddUIComponent<UIButtonExt>().text = "button1";
            AddUIComponent<UIButtonExt>().text = "button2";
        }

        public override void Start()
        {
            base.Start();
            absolutePosition = new Vector2(100, 100);
        }

        protected override void OnSizeChanged()
        {
            if (drag_)
                drag_.width = width;
        }
        private void Drag__eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            // throw new NotImplementedException();
        }
    }
}
