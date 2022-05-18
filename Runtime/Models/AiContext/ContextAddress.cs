using System;

public class ContextAddress
{
       public AiObjectModel Parent{ get; private set; }
       public readonly AiObjectModel AssociatedAiObject;
       public string Address { get; private set; }
       public int Index { get; private set; } = -1;

       public ContextAddress(AiObjectModel associatedAiObject, AiObjectModel parent = null)
       {
              AssociatedAiObject = associatedAiObject;
              Parent = parent;
              UpdateAddress();
       }

       public void SetParent(AiObjectModel parent)
       {
              SetParentAndIndex(parent,Index);
       }

       public void SetIndex(int indexInParent)
       {
              SetParentAndIndex(Parent,indexInParent);
       }
       
       public void SetParentAndIndex(AiObjectModel parent, int indexInParent)
       {
              Parent = parent;
              Index = indexInParent;
              UpdateAddress();
       }

       public void UpdateAddress()
       {
              var address = "";
              if (Parent != null)
              {
                     address += Parent.ContextAddress.Address + "-";
              }

              if (Parent == null)
              {
                     Index = 0;
              }
              Address = address + AssociatedAiObject.BaseAiObjectType.ToString()[..2] + Index;
              
              DebugService.Log("Setting address of: " + AssociatedAiObject.Name + " To: " + Address, this);
       }
}