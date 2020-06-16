using System;
using System.Collections.Generic;

delegate void TreeVisitor<T>(T nodeData);

class NTree<T>
{
    private readonly LinkedList<NTree<T>> _children;

    public T Data { get; private set; }

    public NTree(T data)
    {
        this.Data = data;
        _children = new LinkedList<NTree<T>>();
    }

    public NTree<T> AddChild(T data)
    {
        var newNode = new NTree<T>(data);
        _children.AddFirst(newNode);

        return newNode;
    }

    public NTree<T> GetChildAt(int i)
    {
        foreach (NTree<T> n in _children)
        {
            if (--i == 0)
            {
                return n;
            }
        }

        return null;
    }

    public NTree<T> GetChild(T data)
    {
        foreach (var child in _children)
        {
            if (EqualityComparer<T>.Default.Equals(child.Data, data))
            {
                return child;
            }
        }

        throw new Exception("Child not in node: data: " + data + "; Node Data: " + Data);
    }

    public bool HasChild(T data)
    {
        foreach (var child in _children)
        {
            if (EqualityComparer<T>.Default.Equals(child.Data, data))
            {
                return true;
            }
        }

        return false;
    }

    public void Traverse(NTree<T> node, TreeVisitor<T> visitor)
    {
        visitor(node.Data);
        foreach (NTree<T> child in node._children)
        {
            Traverse(child, visitor);
        }
    }

    public override string ToString()
    {
        return ToString(0);
    }

    public string ToString(int indent)
    {
        var descriptor = new String('-', indent) + "> " + Data + "\n";
        foreach (var child in _children)
        {
            descriptor += child.ToString(indent + 1);
        }

        return descriptor;
    }
}