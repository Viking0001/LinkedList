namespace linkedList;

public class LinkedList<T>
{
    
    public Node<T>? First { get; private set; } = null;
    public Node<T>? Last { get; private set; } = null;
    public int Count { get; private set; } = 0;
    
    
    
    public LinkedList()
    {
        
    }

    public Node<T> AddAfter(Node<T> focus,T value)
    {
        if (AddFirstIfEmpty(value)) return Last!;
        if(focus.Next == null) return AddLast(value);
        
        Node<T> tmp = focus.Next;
        Node<T> node = new Node<T>(value,tmp, focus);
        focus.Next = node;
        tmp.Previous = node;
        Count++;
        return node;
    }

    public Node<T> AddBefore(Node<T> focus, T value)
    {
        if (AddFirstIfEmpty(value)) return First!;
        if(focus.Previous == null) return AddFirst(value);
        
        Node<T> tmp = focus.Previous;
        Node<T> node = new Node<T>(value, focus, tmp);
        focus.Previous = node;
        tmp.Next = node;
        Count++;
        return node;
    }

    public Node<T> AddLast(T value)
    {
        if (AddFirstIfEmpty(value)) return Last!;
        Node<T> node = new Node<T>(value, null, Last);
        Last!.Next = node;
        Last = node;
        Count++;
        return node;
    }
    
    public Node<T> AddFirst(T value)
    {
        if (AddFirstIfEmpty(value)) return First!;
        Node<T> node = new Node<T>(value, First, null);
        First!.Previous = node;
        First = node;
        Count++;
        return node;
    }

    public void Remove(Node<T>? focus)
    {
        if (focus == null) return;
        
        if (focus != First)
        {
            focus.Previous!.Next = focus.Next;
        }
        else
        {
            First = focus.Next;
        }

        if (focus != Last)
        {
            focus.Next!.Previous = focus.Previous;
        }
        else
        {
            Last = focus.Previous;
        }
        
        Count--;
    }

    public void RemoveLast()
    {
        Remove(Last);
    }

    public void RemoveFirst()
    {
        Remove(Last);
    }
    

    private bool AddFirstIfEmpty(T value)
    {
        if (First == null)
        {
            First = new Node<T>(value, null, null);
            Last = First;
            Count++;
            return true;
        }
        return false;
    }


    public override string ToString()
    {
        Node<T>? index = First;
        var result = new System.Text.StringBuilder();

        while (index != null)
        {
            result.Append(index.Value);
            if (index.Next != null)
                result.Append(",");
            index = index.Next;
        }
        return result.ToString();
    }


    public class Node<T2>
    {
        public T2 Value;
        public Node<T2>? Next;
        public Node<T2>? Previous;
        internal Node(T2 value, Node<T2>? next, Node<T2>? previous)
        {
            Value = value;
            Next = next;
            Previous = previous;
        }
    }
}