import { FormEvent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import "../App.css";

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (password !== repeatPassword) {
      setError("Passwords must match.");
      return;
    }
    setLoading(true);
    setError(null);
    try {
      await register(email, password, repeatPassword);
      localStorage.removeItem("retask-video-seen");
      navigate("/welcome");
    } catch (err: any) {
      setError(err?.response?.data?.message ?? "Registration failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <div className="card" style={{ width: "460px" }}>
        <h2 style={{ marginTop: 0, marginBottom: 8 }} className="purple-text">
          Create your account
        </h2>
        <p style={{ marginTop: 0, color: "#5c4a73" }}>
          Join and start delegating tasks to the AI agent.
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
          <div>
            <label className="label" htmlFor="repeatPassword">
              Repeat password
            </label>
            <input
              id="repeatPassword"
              className="input"
              type="password"
              value={repeatPassword}
              onChange={(e) => setRepeatPassword(e.target.value)}
              required
            />
          </div>
          {error && <div className="error">{error}</div>}
          <button className="primary-btn" type="submit" disabled={loading}>
            {loading ? "Creating..." : "Register"}
          </button>
        </form>

        <p style={{ marginTop: 16, fontSize: 14 }}>
          Already have an account?{" "}
          <Link to="/login" className="purple-text">
            Log in
          </Link>
        </p>
      </div>
    </div>
  );
}

