import render from "./render.js";
import storeProxy from "./store.js";
import { addTask, deleteTask, toggleElement } from "./store.js";

document.addEventListener("todosChange", () => {
  console.log("Todo list has changed");
  render();
});

const localElements = JSON.parse(localStorage.getItem("store"));
if (localElements?.todos.length > 0) {
  storeProxy.todos = localElements.todos;
  render();
} else {
  localStorage.setItem("store", JSON.stringify(storeProxy.todos));
  render();
}

const form = document.querySelector("#form");
const formInput = document.querySelector("#new-task");

form.addEventListener("submit", (e) => {
  e.preventDefault();
  const newObj = {
    id: `${Date.now()}-${Math.floor(Math.random() * 1000)}`,
    text: formInput.value,
    completed: false,
  };
  addTask(newObj);
  formInput.value = "";
  render();
});

const list = document.querySelector(".task-list");

list.addEventListener("click", (e) => {
  const target = e.target;
  if (target.classList.contains("btn-cancel")) {
    const id = target.closest(".todo").dataset.id;
    deleteTask(id);
    render();
  }
});

list.addEventListener("change", (e) => {
  const target = e.target;
  if (target.classList.contains("tick")) {
    const id = target.closest(".todo").dataset.id;
    const completed = target.checked;
    toggleElement(id, completed);
    render();
  }
});

render();
