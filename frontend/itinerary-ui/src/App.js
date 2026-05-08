import { useState } from "react";

function App() {
  const [input, setInput] = useState("");
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const API_URL = "https://itineraryparser.onrender.com"; 

  const samples = [
    "2 adults and 1 child traveling from Chennai to Bali between June 10 and June 16 with budget 1.5 lakh INR",
    "Family of 4 traveling from Mumbai to Singapore in August with budget 2L",
    "Trip from Bangalore to Goa for 2 adults, budget 50000 INR",
    "Couple planning a honeymoon from Delhi to Maldives next month with budget 2 lakh INR"
  ];

  const handleParse = async () => {
    setLoading(true);
    setError("");
    setResult(null);

    try {
      const res = await fetch(`${API_URL}/api/itinerary/parse`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ text: input })
      });

      const data = await res.json();

      if (!res.ok) {
        throw new Error(data.error || "Something went wrong");
      }

      setResult(data);
    } catch (err) {
      setError(err.message);
    }

    setLoading(false);
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.title}>🧳 Itinerary Parser</h1>
      <p style={styles.subtitle}>
        Convert travel text into structured data using AI
      </p>

      {/* Sample Buttons */}
      <div style={styles.sampleContainer}>
        {samples.map((s, i) => (
          <button
            key={i}
            style={styles.sampleButton}
            onClick={() => setInput(s)}
          >
            Sample {i + 1}
          </button>
        ))}
      </div>

      {/* Input */}
      <textarea
        placeholder="Paste your travel text here..."
        value={input}
        onChange={(e) => setInput(e.target.value)}
        style={styles.textarea}
      />

      {/* Parse Button */}
      <button
        onClick={handleParse}
        disabled={!input || loading}
        style={{
          ...styles.button,
          opacity: !input || loading ? 0.6 : 1
        }}
      >
        {loading ? "Parsing..." : "Parse"}
      </button>

      {/* Error */}
      {error && <div style={styles.error}>❌ {error}</div>}

      {/* Result Card */}
      {result && (
        <div style={styles.card}>
          <h3>✨ Parsed Result</h3>
          <p><b>From:</b> {result.sourceCity || "-"}</p>
          <p><b>To:</b> {result.destination || "-"}</p>
          <p><b>Start Date:</b> {result.startDate || "-"}</p>
          <p><b>End Date:</b> {result.endDate || "-"}</p>
          <p><b>Adults:</b> {result.adults ?? "-"}</p>
          <p><b>Children:</b> {result.children ?? "-"}</p>
          <p><b>Budget:</b> {result.budget ?? "-"} {result.currency || ""}</p>
        </div>
      )}
    </div>
  );
}

/* ================= STYLES ================= */

const styles = {
  container: {
    maxWidth: "700px",
    margin: "40px auto",
    padding: "20px",
    fontFamily: "Arial, sans-serif",
    textAlign: "center"
  },
  title: {
    marginBottom: "5px"
  },
  subtitle: {
    color: "#666",
    marginBottom: "20px"
  },
  sampleContainer: {
    marginBottom: "15px"
  },
  sampleButton: {
    margin: "5px",
    padding: "8px 12px",
    cursor: "pointer",
    borderRadius: "6px",
    border: "1px solid #ccc",
    background: "#f5f5f5"
  },
  textarea: {
    width: "100%",
    height: "120px",
    padding: "10px",
    marginBottom: "10px",
    borderRadius: "6px",
    border: "1px solid #ccc"
  },
  button: {
    padding: "10px 20px",
    cursor: "pointer",
    borderRadius: "6px",
    border: "none",
    background: "#007bff",
    color: "white",
    marginBottom: "15px"
  },
  error: {
    color: "red",
    marginTop: "10px"
  },
  card: {
    textAlign: "left",
    background: "#f9f9f9",
    padding: "15px",
    borderRadius: "8px",
    marginTop: "20px",
    border: "1px solid #ddd"
  }
};

export default App;