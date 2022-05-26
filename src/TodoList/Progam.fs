namespace TodoList

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App = 
   type Todo = { 
       Task: string 
   }

   type Model = {
       Todos: Todo list
       EntryText: string
   }

   type Msg = 
       | AddTodoFromEntryText
       | RemoveTodo of Todo
       | EntryTextChanged of string

   let initModel = { Todos = []; EntryText = ""}

   let init () = initModel

   let update msg model =
       match msg with
       | AddTodoFromEntryText ->
           let newTodo = { Task = model.EntryText }
           { model with Todos = newTodo::model.Todos; EntryText = ""}
       
       | RemoveTodo todo ->
           { model with Todos = model.Todos |> List.filter (fun t -> t <> todo)}
       
       | EntryTextChanged newTodoText ->
           { model with EntryText = newTodoText }

   let view model dispatch =
       View.ContentPage(
           View.StackLayout(
               padding = Thickness(20., 0.),
               children = [
                  View.Label(
                   text = "My To-Dos",
                   fontSize = FontSize.fromNamedSize NamedSize.Title,
                   horizontalOptions = LayoutOptions.Center
                  )

                  //Add new todo
                  View.Grid( 
                   coldefs = [Star; Auto],
                   margin = 5.0,
                   children = [
                       View.Entry(
                           text = model.EntryText,
                           textChanged = fun e -> dispatch (EntryTextChanged e.NewTextValue)
                       )
                       
                       View.Button(
                           text = "Add todo",
                           command = fun () -> dispatch AddTodoFromEntryText
                       ).Column(1)
                   ]
                  )
                  
                  View.ListView([
                    for todo in model.Todos -> 
                        View.ViewCell( key = todo.Task,
                                view = View.SwipeView( leftItems = 
                                        View.SwipeItems(items = [
                                            View.SwipeItem(
                                                text="delete", 
                                                backgroundColor = Color.IndianRed,
                                                command = fun () -> dispatch (RemoveTodo todo)
                                            )
                                        ]),
                                content = 
                                    View.StackLayout(
                                        backgroundColor = Color.LightGray,
                                        children = [View.Label(text = todo.Task)]
                                    )
                                )
                        )
                  ])
            ])
       )

   let program = XamarinFormsProgram.mkSimple init update view

type App () as app = 
   inherit Application ()

   let runner =
       App.program
       |> Program.withConsoleTrace
       |> XamarinFormsProgram.run app

#if DEBUG
   do runner.EnableLiveUpdate()
#endif


