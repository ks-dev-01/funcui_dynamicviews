# Using Avalonia FuncUI to create resizable panels

Looks at using FuncUI to create a screen with resizable views and scroll bars.

The view will look like:

- Base Layer - Two Columns
    - Column 1 - Contains Two Lists
        - A splitter that can be dragged up and down, changing the size of each list panel, and the scroll bars
    - Column 2 - Dyanimc list of panels
        - Each Panel can be resize vertically
        - Each panel can have a list of items
        - If the list of items exceeds the panel size it will have a scroll bar
        - When the a panel is resized - the entire are is resized
    - Between these columns will be a Vertical Splitter that can be drag horzontially, changing the sizes of Column 1 and Column 2


