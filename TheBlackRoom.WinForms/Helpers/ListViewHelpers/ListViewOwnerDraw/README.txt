Reasons:
  OwnerDrawing a listview is hard
  Would be nice to just ownerdraw a single view without having to ownerdraw everything
  Would be nice to ownerdraw a single details column
  Can't change the default highlight colours

BUGS:
  [View==Details && ShowItemTooltips==True && FullRowSelect==True OwnerDraw==True]
    Hovering over any column of a selected item clears the contents of the other columns
    This does not occur when ShowItemTooltips==false or FullRowSelect==false
    This does occur when the listview is focused or not focused

Differences from stock listview:
  Selection rectangle and focus rectangle is full item bounds, not just the text bounds
  Tile text is always the full width (see below)
  Tile text is dual colored when listview is unfocused but the item is still selected
     (instead of being the same colour)
  Slightly more text is shown when using DrawText() compared to stock listview
  
  [View==Tile]
     sometimes the width of secondary text lines are smaller than the tile width
     (they match the width of the first line) but not always?

TODO: Add support for checkboxes in ownerdraw (use CheckBoxRenderer.DrawCheckBox())
TODO: Add support for StateImageList
TODO: Use LabelWrap property for wrapping text flags
TODO: Fix drawing column header images when column has center or right alignment
TODO: Column Headers don't have mouseover highlighting, when using
      DrawListViewColumnHeaderEventArgs.DrawBackground() (So this is a ListView bug)
