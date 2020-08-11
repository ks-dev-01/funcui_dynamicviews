# Explore resizable panels in Avalonia FuncUI

Explore the best way to achieve the below mock-up using FuncUI.

The view will look like:
![alt text](https://github.com/sharp-fsh/funcui_dynamicviews/blob/master/view_template_example.png)

- Base Layer - Two Columns
    - Column 1 - Contains Two Lists
        - A splitter that can be dragged up and down, changing the size of each list panel, and the scroll bars
    - Column 2 - Dynamic list of panels
        - Each Panel can be resized vertically
        - Each panel can have a list of items
        - If the list of items exceeds the panel size it will have a scroll bar
    - Between these Column 1 and 2 will be a splitter that can be dragged left and right, changing the sizes of Column 1 and Column 2
    - Also allow the View State to be saved - so all resizable values will be stored in state (which would later be saved in a database)
    
## Attempt 1

Used a Grid Layout for the first component - Two Vertical Lists in one column.
Initially this worked and the Grid Splitter would move the other two cells, and the scrollbars would react.
But when the data in the lists change the Grid Splitter function no longer works.

**COULD NOT GET THIS TO WORK**

## Attempt 2
Custom Splitter. Use a control with drag and drop events that track position and change row definitions when mouse moves.
This currently works - need to work on making the drag feel smoother - get the delta right.

## Initial Thoughts

- For Base Layer use a Grid layout with 3 columns where:
    - Column 1 holds the Lists
    - Column 2 holds a Grid Splitter
    - Column 3 holds the panels
    
- What would be the best columnDefinitions for the Base Layer Grid?
    - Dynamic values:
        - 0.4*, 0.09 (GridSplitter), 0.6*
    - Or should the be pixel widths?
        - 400, 5, 600
- How does the GridSplitter work? Would it work in FuncUI & Elmish?

        


