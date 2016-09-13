using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.PathFinding {
    public class PriorityQueue<T> {
        private List<T> items = new List<T>();
        private List<double> priorities = new List<double>();
        private int Parent(int i) {
            return (i + 1) / 2 - 1;
        }
        private int LeftChild(int i) {
            return (i + 1) * 2 - 1;
        }
        private int RightChild(int i) {
            return (i + 1) * 2;
        }

        private void FloatUp(int i) {
            if (i == 0 || priorities[i] > priorities[Parent(i)])
                return;

            Swap(i, Parent(i));
            FloatUp(Parent(i));
        }

        private void SinkDown(int i) {
            int leftChildIndex = LeftChild(i);
            int rightChildIndex = RightChild(i);
            int highestPriorityIndex = i;

            if (leftChildIndex < items.Count
                && priorities[leftChildIndex] < priorities[highestPriorityIndex]) {
                highestPriorityIndex = leftChildIndex;
            }

            if (rightChildIndex < items.Count
                && priorities[rightChildIndex] < priorities[highestPriorityIndex]) {
                highestPriorityIndex = rightChildIndex;
            }

            if (highestPriorityIndex == i)
                return;

            Swap(i, highestPriorityIndex);
            SinkDown(highestPriorityIndex);
        }

        public void Add(T item, double priority) {
            items.Add(item);
            priorities.Add(priority);
            FloatUp(items.Count-1);
        }

        public T Pop() {
            T ret = items[0];
            items.RemoveAt(0);
            priorities.RemoveAt(0);
            SinkDown(0);
            return ret;
        }

        public T Peek() {
            return items[0];
        }

        private void Swap(int i, int j) {
            T tempItem = items[i];
            items[i] = items[j];
            items[j] = tempItem;

            double tempPriority = priorities[i];
            priorities[i] = priorities[j];
            priorities[j] = tempPriority;
        }

        public int Count() {
            return items.Count;
        }
    }
}
