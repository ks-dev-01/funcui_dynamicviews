namespace DynamicViews

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Components
    open Avalonia.Controls.Primitives
    
    type State = { 
        ListOneItems : string list 
        ListTwoItems : string list }

    let generateStrings total word = 
        [
            for i in 0..total-1 do
                sprintf "%s: %i" word (i+1)
        ]

    let init = { 
        ListOneItems = generateStrings 20 "List One" 
        ListTwoItems = generateStrings 30 "List Two" 
    }

    type Msg = 
    | ChangeItems

    let update (msg: Msg) (state: State) : State =
        match msg with
        | ChangeItems -> { state with ListOneItems = generateStrings 5 "Update List One"
                                      ListTwoItems = generateStrings 15 "Update List Two" }
    
    let buttonPanel gridPostion dispatch = 
        StackPanel.create [
            gridPostion
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [
                Button.create [
                    Button.content "Change Lists"
                    Button.width 50.
                    Button.height 35.
                    Button.onClick (fun _ -> dispatch ChangeItems)
                ]
            ]
        ]
        
    let genericListBox (items : string list) = 
        ListBox.create  [
                ListBox.dataItems items
                ListBox.itemTemplate (
                    DataTemplateView<string>.create (fun (text) ->
                        TextBlock.create [ TextBlock.text text ]
                    )
                )
            ]

    let verticalScrollerWithItems gridPostion items =
        ScrollViewer.create     [
            gridPostion
            ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Auto
            ScrollViewer.content (             
               genericListBox items
            )
        ]

    let verticalStackPanelWithItems gridPostion items =
        StackPanel.create [
            gridPostion
            StackPanel.orientation Orientation.Vertical
            StackPanel.children [
                genericListBox items
            ]
        ]

    let view (state: State) (dispatch) =
        Grid.create [
            Grid.rowDefinitions "35, 1*, 5, 4*"
            Grid.showGridLines true
            Grid.children [
                buttonPanel (Grid.row 0) dispatch
                verticalScrollerWithItems (Grid.row 1) state.ListOneItems
                GridSplitter.create [(Grid.row 2); GridSplitter.horizontalAlignment HorizontalAlignment.Stretch ]
                verticalScrollerWithItems (Grid.row 3) state.ListTwoItems
            ]
        ]