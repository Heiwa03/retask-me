import { FormEvent, useState } from "react";
import { askAgent } from "../api/ai";
import "./ChatPanel.css";

interface Message {
  from: "user" | "agent";
  text: string;
}

interface Props {
  token: string;
  onAfterAgentResponse?: () => Promise<void> | void;
}

export default function ChatPanel({ token, onAfterAgentResponse }: Props) {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSend = async (e: FormEvent) => {
    e.preventDefault();
    if (!input.trim()) return;
    const userMessage: Message = { from: "user", text: input.trim() };
    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    setLoading(true);
    setError(null);
    try {
      const result = await askAgent(token, userMessage.text);
      setMessages((prev) => [...prev, { from: "agent", text: result }]);
      if (onAfterAgentResponse) {
        await onAfterAgentResponse();
      }
    } catch (err: any) {
      setError(err?.message ?? "Agent unavailable");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="chat-panel">
      <div className="chat-panel__header">
        <div>
          <p className="chat-title">AI Agent</p>
          <p className="chat-subtitle">Delegate tasks or ask for help</p>
        </div>
        <span className={`status-dot ${loading ? "status-dot--busy" : ""}`} />
      </div>

      <div className="chat-panel__messages">
        {messages.length === 0 && (
          <div className="chat-placeholder">
            <p>Try asking: “Draft a plan for my next sprint.”</p>
          </div>
        )}
        {messages.map((msg, idx) => (
          <div key={idx} className={`bubble bubble--${msg.from}`}>
            {msg.text}
          </div>
        ))}
      </div>

      <form className="chat-panel__input" onSubmit={handleSend}>
        <input
          className="input"
          placeholder="Ask the agent..."
          value={input}
          onChange={(e) => setInput(e.target.value)}
          disabled={loading}
        />
        <button className="primary-btn" type="submit" disabled={loading}>
          {loading ? "Thinking..." : "Send"}
        </button>
      </form>

      {error && <div className="error">{error}</div>}
    </div>
  );
}

