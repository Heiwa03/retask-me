import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import TaskList from "../components/TaskList";
import TaskForm from "../components/TaskForm";
import ChatPanel from "../components/ChatPanel";
import { createTask, fetchTasks, updateTask, deleteTask } from "../api/tasks";
import { getBoards, createBoard, addTaskToBoard, removeTaskFromBoard } from "../api/boards";
import { Task, Board } from "../types";
import { useAuth } from "../hooks/useAuth";
import { Link } from "react-router-dom";
import "../App.css";
import "../components/TaskList.css";
import "../components/TaskForm.css";
import "../components/ChatPanel.css";

export default function DashboardPage() {
  const { token, logout } = useAuth();
  const navigate = useNavigate();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [boards, setBoards] = useState<Board[]>([]);
  const [selectedBoard, setSelectedBoard] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [creatingBoard, setCreatingBoard] = useState(false);
  const [newBoardTitle, setNewBoardTitle] = useState("");
  const [newBoardDescription, setNewBoardDescription] = useState("");

  const reloadTasks = async () => {
    if (!token) return;
    const data = await fetchTasks(token);
    setTasks(data);
  };

  const reloadBoards = async () => {
    if (!token) return;
    const data = await getBoards(token);
    setBoards(data);
  };

  useEffect(() => {
    if (!token) return;
    (async () => {
      try {
        await reloadTasks();
        await reloadBoards();
      } catch (err: any) {
        setError(err?.response?.data?.message ?? "Failed to load tasks");
      } finally {
        setLoading(false);
      }
    })();
  }, [token]);

  const handleCreateTask = async (payload: {
    title: string;
    description?: string;
    deadline?: string;
    priority: number;
    status: number;
  }) => {
    if (!token) return;
    await createTask(token, payload);
    await reloadTasks();
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const handleCreateBoard = async () => {
    if (!token || !newBoardTitle.trim()) return;
    setCreatingBoard(true);
    setError(null);
    try {
      await createBoard(token, { title: newBoardTitle.trim(), description: newBoardDescription.trim() });
      setNewBoardTitle("");
      setNewBoardDescription("");
      await reloadBoards();
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Failed to create board");
    } finally {
      setCreatingBoard(false);
    }
  };

  const handleAssignBoard = async (taskUid: string, boardUuid: string) => {
    if (!token) return;
    await addTaskToBoard(token, boardUuid, taskUid);
    await reloadTasks();
  };

  const handleRemoveBoard = async (taskUid: string) => {
    if (!token) return;
    const task = tasks.find((t) => t.uuid === taskUid);
    if (!task || !task.boardUuid) return;
    await removeTaskFromBoard(token, task.boardUuid, taskUid);
    await reloadTasks();
  };

  const handleEditTask = async (
    taskUid: string,
    update: { title: string; description?: string; deadline?: string | null; priority: number; status: number },
  ) => {
    if (!token) return;
    await updateTask(token, taskUid, {
      title: update.title,
      description: update.description ?? "",
      deadline: update.deadline,
      priority: update.priority,
      status: update.status,
    });
    await reloadTasks();
  };

  const handleDeleteTask = async (taskUid: string) => {
    if (!token) return;
    await deleteTask(token, taskUid);
    await reloadTasks();
  };

  const handleMarkComplete = async (taskUid: string) => {
    if (!token) return;
    const task = tasks.find((t) => t.uuid === taskUid);
    if (!task) return;
    await updateTask(token, taskUid, {
      title: task.title,
      description: task.description ?? "",
      deadline: task.deadline ?? null,
      priority: task.priority,
      status: 3, // Done
    });
    await reloadTasks();
  };

  const filteredTasks = selectedBoard
    ? tasks.filter((t) => t.boardUuid === selectedBoard)
    : tasks;

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        flexDirection: "column",
        padding: "24px",
        gap: "16px",
      }}
    >
      <header
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <div>
          <h1 style={{ margin: 0, color: "var(--purple-900)" }}>ReTask-Me</h1>
          <p style={{ margin: 0, color: "#5c4a73" }}>
            Sorting all your problems with AI since 2025.
          </p>
        </div>
        <div style={{ display: "flex", gap: 10 }}>
          <Link className="secondary-btn" to="/profile">
            Profile
          </Link>
          <button className="secondary-btn" onClick={handleLogout}>
            Log out
          </button>
        </div>
      </header>

      {error && <div className="error">{error}</div>}

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "2fr 1fr",
          gap: "18px",
          alignItems: "stretch",
        }}
      >
        <div
          style={{
            display: "grid",
            gridTemplateRows: "auto 1fr",
            gap: "16px",
            minHeight: "70vh",
          }}
        >
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px", alignItems: "stretch" }}>
            <div
              className="card"
              style={{
                display: "grid",
                gap: 12,
                height: "100%",
              }}
            >
              <h3 style={{ margin: 0, fontSize: "18px", color: "var(--purple-900)" }}>Board filter</h3>
              <p style={{ margin: 0, fontStyle: "italic", color: "#5c4a73" }}>
                Idea courtesy of Mr. Andrei Medvediev
              </p>
              <div>
                <label className="label">Filter by board</label>
                <select
                  className="input"
                  value={selectedBoard}
                  onChange={(e) => setSelectedBoard(e.target.value)}
                >
                  <option value="">All tasks</option>
                  {boards.map((b) => (
                    <option key={b.uuid} value={b.uuid}>
                      {b.title}
                    </option>
                  ))}
                </select>
              </div>
              <div style={{ display: "grid", gap: 8 }}>
                <label className="label">Create board</label>
                <input
                  className="input"
                  placeholder="Board title"
                  value={newBoardTitle}
                  onChange={(e) => setNewBoardTitle(e.target.value)}
                />
                <input
                  className="input"
                  placeholder="Board description (optional)"
                  value={newBoardDescription}
                  onChange={(e) => setNewBoardDescription(e.target.value)}
                />
                <button className="primary-btn" type="button" disabled={creatingBoard} onClick={handleCreateBoard}>
                  {creatingBoard ? "Creating..." : "Create board"}
                </button>
              </div>
            </div>

            <div className="card" style={{ height: "100%", display: "grid", gap: 8 }}>
              <h3 style={{ margin: 0, fontSize: "18px", color: "var(--purple-900)" }}>Create task</h3>
              <TaskForm onCreate={handleCreateTask} />
            </div>
          </div>

          <div className="card" style={{ overflow: "auto", display: "grid", gap: 8 }}>
            <h3 style={{ margin: 0, fontSize: "18px", color: "var(--purple-900)" }}>Task list</h3>
            {loading ? (
              <p>Loading tasks...</p>
            ) : (
              <TaskList
                tasks={filteredTasks}
                boards={boards}
                onEdit={handleEditTask}
                onDelete={handleDeleteTask}
                onMarkComplete={handleMarkComplete}
                onAssignBoard={handleAssignBoard}
                onRemoveBoard={handleRemoveBoard}
              />
            )}
          </div>
        </div>

        <div style={{ height: "100%" }}>
          <ChatPanel token={token!} onAfterAgentResponse={reloadTasks} />
        </div>
      </div>
    </div>
  );
}

