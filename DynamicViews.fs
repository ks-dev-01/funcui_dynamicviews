namespace DynamicViews

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Components
    open Avalonia.Controls.Primitives
    open Avalonia.Input
    open Avalonia.VisualTree
    open Avalonia

    type DragState = 
    | Not
    | Dragging

    type State = { 
        ListOneItems : string list 
        ListTwoItems : string list 
        RowDefs : string 
        DragState : DragState 
        OldDragPoint : Point option
        RowDef1 : float
        RowDef2 : float }

    let generateStrings total word = 
        [
            for i in 0..total-1 do
                sprintf "%s: %i" word (i+1)
        ]

    let rowDefinition1 = "35, 100*, 5, 400*"
    let rowDefinition2 = "35, 300*, 5, 400*"
    
    let init = { 
        ListOneItems = generateStrings 20 "List One" 
        ListTwoItems = generateStrings 30 "List Two" 
        RowDefs = rowDefinition1
        DragState = Not
        OldDragPoint = None
        RowDef1 = 100.
        RowDef2 = 400.
    }

    type Msg = 
    | ChangeItems
    | ChangeGridRowDefsButton
    | GridRowDefinitionsChanged of string
    | Nothing
    | CustomSplitterDragStart of PointerPressedEventArgs
    | CustomSplitterDragDelta of PointerEventArgs
    | CustomSplitterDragEnd of PointerReleasedEventArgs

    let update (msg: Msg) (state: State) : State =
        match msg with
        | ChangeItems -> { state with ListOneItems = generateStrings 5 "Update List One"
                                      ListTwoItems = generateStrings 15 "Update List Two" }
        | ChangeGridRowDefsButton -> if state.RowDefs = rowDefinition2 then { state with RowDefs = rowDefinition1 } else { state with RowDefs = rowDefinition2 }
        | GridRowDefinitionsChanged stringDef -> state //{ state with RowDefs = stringDef }
        | CustomSplitterDragStart event -> { state with DragState = Dragging }
        | CustomSplitterDragDelta event ->
            match state.DragState with
            | Not -> state
            | Dragging ->
                let newPoint = (event.GetPosition ((event.Source :?> IVisual)))
                printfn "New Point: %A Old Point: %A" newPoint state.OldDragPoint
                match state.OldDragPoint with
                | None -> { state with OldDragPoint = Some newPoint }
                | Some oldPoint -> 
                    let dX = newPoint.X - oldPoint.X
                    let dY = newPoint.Y - oldPoint.Y
                    
                    let newRowDef1 = state.RowDef1 + dY
                    let newRowDef2 = state.RowDef2 - dY

                    let rowDefs = sprintf "35, %f*, 5, %f*" newRowDef1 newRowDef2
                    { state with OldDragPoint = Some newPoint; RowDef1 = newRowDef1; RowDef2 = newRowDef2; RowDefs = rowDefs }
        | CustomSplitterDragEnd event -> { state with DragState = Not }
        | Nothing -> state
    
    let buttonPanel gridPosition dispatch = 
        StackPanel.create [
            gridPosition
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [
                Button.create [
                    Button.content "Change Lists"
                    Button.width 120.
                    Button.height 35.
                    Button.onClick (fun _ -> dispatch ChangeItems)
                ]
                Button.create [
                    Button.content "Change Row Definitions"
                    Button.width 120.
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

    let customSplitter gridPosition dispatch =
        Border.create [
            gridPosition
            Border.height 3.
            Border.background "pink"
            Border.cursor (Cursor StandardCursorType.SizeNorthSouth)
            Border.onPointerPressed (fun e -> dispatch (CustomSplitterDragStart e))
            Border.onPointerReleased (fun e -> dispatch (CustomSplitterDragEnd e))
            Border.onPointerMoved (fun e -> dispatch (CustomSplitterDragDelta e))
        ]


    let gridSplitterComponent gridPosition dispatch = 
        GridSplitter.create [
                gridPosition
                GridSplitter.horizontalAlignment HorizontalAlignment.Stretch 
                GridSplitter.onDragDelta (fun e ->
                    try 
                        let source = (e.Source :?> Control)
                        let parent = source.Parent
                        let parentAsGrid = parent :?> Grid
                        let rowDefs = parentAsGrid.RowDefinitions
                        let stringDef =
                            [ for rowDef in rowDefs do
                                sprintf "%A" rowDef.Height
                            ] |> String.concat ","
                        printfn "RowDefs: %A" stringDef
                        (dispatch (GridRowDefinitionsChanged stringDef))
                    with ex ->
                        printfn "Error: %A" ex
                    )

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
                        printfn "RowDefs: %A" stringDef
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
                //gridSplitterComponent (Grid.row 2) dispatch
                customSplitter (Grid.row 2) dispatch
                verticalScrollerWithItems (Grid.row 3) state.ListTwoItems
            ]
        ]
