using System;
using System.Collections.Generic;

namespace GracesGames.Common.Scripts {

    [Serializable]
    public class FiniteStack<T> : LinkedList<T> {

            private const int Size = 10;

            public T Peek() {
            return Last.Value;
        }

        public T Pop() {
            LinkedListNode<T> node = Last;

            if (node != null) {
	            RemoveLast();
	            return node.Value;
            }
            return default(T);
        }

        public void Push(T value) {
            LinkedListNode<T> node = new LinkedListNode<T>(value);

            AddLast(node);

            if (Count > Size) {
	            RemoveFirst();
            }
        }
    }
}
