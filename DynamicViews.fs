namespace DynamicViews

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Components
    open Avalonia.Controls.Primitives
    
    type State = { 
        ListOneItems : string list 
        ListTwoItems : string list 
        RowDefs : string }

    let generateStrings total word = 
        [
            for i in 0..total-1 do
                sprintf "%s: %i" word (i+1)
        ]

    let rowDefinition1 = "35, 1*, 5, 4*"
    let rowDefinition2 = "35, 400*, 5, 400*"
    
    let init = { 
        ListOneItems = generateStrings 20 "List One" 
        ListTwoItems = generateStrings 30 "List Two" 
        RowDefs = "35, 1*, 5, 4*"
    }

    type Msg = 
    | ChangeItems
    | ChangeGridRowDefsButton
    | GridRowDefinitionsChanged of string
    | Nothing

    let update (msg: Msg) (state: State) : State =
        match msg with
        | ChangeItems -> { state with ListOneItems = generateStrings 5 "Update List One"
                                      ListTwoItems = generateStrings 15 "Update List Two" }
        | ChangeGridRowDefsButton -> if state.RowDefs = rowDefinition2 then { state with RowDefs = rowDefinition1 } else { state with RowDefs = rowDefinition2 }
        | GridRowDefinitionsChanged stringDef -> { state with RowDefs = stringDef }
        | Nothing -> state
    
    let buttonPanel gridPosition dispatch = 
        StackPanel.create [
            gridPosition
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [
                Button.create [
                    Button.content "Change Lists"
                    Button.width 50.
                    Button.height 35.
                    Button.onClick (fun _ -> dispatch ChangeItems)
                ]
                Button.create [
                    Button.content "Change Row Definitions"
                    Button.width 50.
                    Button.height 35.
                    Button.onClick (fun _ -> dispatch ChangeGridRowDefsButton)
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

    let gridSplitterComponent gridPosition dispatch = 
        GridSplitter.create [
                gridPosition
                GridSplitter.horizontalAlignment HorizontalAlignment.Stretch 
                GridSplitter.onDragCompleted (fun e ->
                    try 
                        let source = (e.Source :?> Control)
                        let parent = source.Parent
                        let parentAsGrid = parent :?> Grid
                        let rowDefs = parentAsGrid.RowDefinitions
                        let stringDef =
                            [ for rowDef in rowDefs do
                                sprintf "%A" rowDef.Height
                            ] |> String.concat ","
                        (dispatch (GridRowDefinitionsChanged stringDef))
                    with ex ->
                        printfn "Error: %A" ex
                    )
        ]
        
    let view (state: State) (dispatch) =
        Grid.create [
            Grid.rowDefinitions state.RowDefs
            Grid.showGridLines true
            Grid.children [
                buttonPanel (Grid.row 0) dispatch
                verticalScrollerWithItems (Grid.row 1) state.ListOneItems
                gridSplitterComponent (Grid.row 2) dispatch
                verticalScrollerWithItems (Grid.row 3) state.ListTwoItems
            ]
        ]
