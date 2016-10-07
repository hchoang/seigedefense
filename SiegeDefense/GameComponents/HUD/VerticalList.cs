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
        public List<ListItem<T>> items { get; set; }
        public VerticalList(List<ListItem<T>> items, Vector2 position, Vector2 size, Vector2 padding, int nVisibleItems, Texture2D frameSprite) {
            this.items = items;

            // frame
            renderer = new SpriteRenderer(frameSprite);
            renderer.position = position;
            renderer.size = size;
            AddComponent(renderer);

            // items
            Vector2 itemSize = new Vector2(1 - padding.X * 2, (1 - padding.Y / 2) / nVisibleItems);
            float itemX = padding.X;
            for (int i=0; i<items.Count; i++) {
                float itemY = itemX + (i * itemSize.Y);
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
    }
}
