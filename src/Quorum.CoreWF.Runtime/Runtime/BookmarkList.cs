// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace System.Activities.Runtime;

[DataContract]
public class BookmarkList : HybridCollection<Bookmark>
{
    public BookmarkList()
        : base() { }

    public bool Contains(Bookmark bookmark)
    {
        if (SingleItem != null)
        {
            if (SingleItem.Equals(bookmark))
            {
                return true;
            }
        }
        else if (MultipleItems != null)
        {
            for (int i = 0; i < MultipleItems.Count; i++)
            {
                if (bookmark.Equals(MultipleItems[i]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void TransferBookmarks(out Bookmark singleItem, out IList<Bookmark> multipleItems)
    {
        singleItem = SingleItem;
        multipleItems = MultipleItems;
    }

}
