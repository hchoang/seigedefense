using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public class VerticalList<T> : HUD {

        public Action<HUD, T> onItemSelected { get; set; }
        public List<ListItem<T>> items { get; set; } = new List<ListItem<T>>();
        public float scrollPosition { get; set; } = 0;
        public float maxScrollPosition { get; set; }
        public Vector2 padding { get; set; }
        public float nVisibleItems { get; set; }
        public Vector2 itemSize { get; set; }
        public Rectangle scissorRect { get; set; }
        public RasterizerState clippingRS { get; set; }

        public VerticalList(_2DRenderer renderer, Vector2 padding, float nVisibleItems) {
            this.padding = padding;
            this.nVisibleItems = nVisibleItems;
            this.renderer = renderer;
            this.renderer.AddChildRenderer(new _2DRenderer());
            AddComponent(renderer);

            itemSize = new Vector2(1 - padding.X * 2, (1 - padding.Y * 2) / nVisibleItems);
            maxScrollPosition = itemSize.Y * (items.Count - nVisibleItems);
            if (maxScrollPosition < 0) {
                maxScrollPosition = 0;
            }

            scissorRect = Utility.CalculateDrawArea(padding, Vector2.One - padding * 2, renderer.GetDrawArea());
            clippingRS = new RasterizerState();
            clippingRS.ScissorTestEnable = true;
        }

        public void Add(ListItem<T> item) {
            Vector2 itemPosition = padding + new Vector2(0, itemSize.Y * items.Count);
            item.renderer.position = itemPosition;
            item.renderer.size = itemSize;
            item.renderer.SetRasterizerState(clippingRS, true);
            item.renderer.SetScissorRectangle(scissorRect, true);
            item.onClick = itemSelected;
            items.Add(item);
            renderer.childRenderers[0].AddChildRenderer(item.renderer);
            AddComponent(item);

            maxScrollPosition = itemSize.Y * (items.Count - nVisibleItems) * renderer.GetDrawArea().Height;
            if (maxScrollPosition < 0) {
                maxScrollPosition = 0;
            }
        }

        public VerticalList(List<ListItem<T>> items, Vector2 position, Vector2 size, Vector2 padding, float nVisibleItems, Texture2D frameSprite) {
            this.items = items;
            this.padding = padding;

            // frame
            renderer = new SpriteRenderer(frameSprite);
            renderer.position = position;
            renderer.size = size;
            AddComponent(renderer);

            // items
            Vector2 itemSize = new Vector2(1 - padding.X * 2, (1 - padding.Y * 2) / nVisibleItems);
            float itemX = padding.X;
            for (int i=0; i<items.Count; i++) {
                float itemY = padding.Y + (i * itemSize.Y);
                Vector2 itemPosition = new Vector2(itemX, itemY);
                items[i].renderer.position = new Vector2(itemX, itemY);
                items[i].renderer.size = itemSize;
                items[i].onClick = itemSelected;

                renderer.AddChildRenderer(items[i].renderer);
                AddComponent(items[i]);
            }
        }

        public void itemSelected(HUD invoker) {
            onItemSelected?.Invoke(this, ((ListItem<T>)invoker).data);
        }

        public void selectItem(int index) {
            items[index].onClick?.Invoke(items[index]);
        }

        public override void Update(GameTime gameTime) {
            float x = inputManager.GetValue(GameInput.PointerX);
            float y = inputManager.GetValue(GameInput.PointerY);

            if (renderer.GetDrawArea().Contains(x, y)) {
                float scrollValue = inputManager.GetValue(GameInput.Zoom) / 10;
                scrollPosition = MathHelper.Clamp(scrollPosition + scrollValue, 0, maxScrollPosition);
            }

            Vector2 rootPosition = renderer.childRenderers[0].position;
            rootPosition.Y = -scrollPosition / renderer.GetDrawArea().Height;
            renderer.childRenderers[0].position = rootPosition;

            base.Update(gameTime);
        }
    }
}
