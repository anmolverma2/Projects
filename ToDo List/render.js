import store from "./store.js";

function render() {
  var todos = document.querySelector(".task-list");

  const todoElement = store.todos
    .map((todo) => {
      return `<li class='todo' data-id="${todo.id}" data-name="${todo.text}">
                           <span class="${todo.completed ? "completed" : ""}">${
        todo.text
      }</span>
                           <div class="task-controls">
                               <label>
                                   <input class='tick' type="checkbox" ${
                                     todo.completed ? "checked" : ""
                                   }>
                               </label>
                               <button type="button" class="btn-cancel">x</button>
                           </div>
                       </li>`;
    })
    .join("");

  todos.innerHTML = todoElement;
}

export default render;
