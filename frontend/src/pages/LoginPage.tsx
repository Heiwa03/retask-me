import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import "../App.css";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await login(email, password);
      localStorage.removeItem("retask-video-seen");
      navigate("/welcome");
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Login failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <div className="card" style={{ width: "440px" }}>
        <h2 style={{ marginTop: 0, marginBottom: 8 }} className="purple-text">
          Welcome back
        </h2>
        <p style={{ marginTop: 0, color: "#5c4a73" }}>
          Sign in to see your tasks and chat with the agent.
        </p>

        <form onSubmit={handleSubmit} className="form-grid">
          <div>
            <label className="label" htmlFor="email">
              Email
            </label>
            <input
              id="email"
              className="input"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div>
            <label className="label" htmlFor="password">
              Password
            </label>
            <input
              id="password"
              className="input"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          {error && <div className="error">{error}</div>}
          <button className="primary-btn" type="submit" disabled={loading}>
            {loading ? "Signing in..." : "Log in"}
          </button>
        </form>

        <p style={{ marginTop: 16, fontSize: 14 }}>
          New here?{" "}
          <Link to="/register" className="purple-text">
            Create an account
          </Link>
        </p>
      </div>
    </div>
  );
}

