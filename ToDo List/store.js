const store = {
  todos: [
    {
      id: "1",
      text: "Buy milk",
      completed: false,
    },
    {
      id: "2",
      text: "Walk the dog",
      completed: true,
    },
    {
      id: "3",
      text: "Do laundry",
      completed: false,
    },
  ],
};

const storeHandler = {
  get(target, property) {
    return target[property];
  },
  set(target, property, value) {
    if (property === "todos") {
      target[property] = value;
      window.dispatchEvent(new Event("todosChange"));
    } else {
      target[property] = value;
    }
    localStorage.setItem("store", JSON.stringify(store));
    return true;
  },
};
const storeProxy = new Proxy(store, storeHandler);

function addTask(newObj) {
  storeProxy.todos = [...storeProxy.todos, newObj];
}

function deleteTask(id) {
  storeProxy.todos = storeProxy.todos.filter((todo) => todo.id !== id);
}

function toggleElement(id, completed) {
  storeProxy.todos = storeProxy.todos.map((todo) => {
    if (todo.id === id) {
      return { ...todo, completed: completed };
    }
    return todo;
  });
}

export { addTask, deleteTask, toggleElement };

export default storeProxy;
