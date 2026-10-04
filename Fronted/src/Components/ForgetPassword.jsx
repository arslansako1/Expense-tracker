import { useState } from "react";
import { Link } from "react-router-dom";
import { API_ENDPOINTS } from "../Configration/Urls";
import "../Css/ForgetPassword.css";

export default function ForgetPassword() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");


  const handleSubmit = async (e) => {
    
    e.preventDefault();

    setLoading(true);
    setError("");
    setMessage("");

    try {

      const response = await fetch(API_ENDPOINTS.forgetPassword, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email }),
      });


      const data = await response.json();

      if (!response.ok) {
        setError(data.message || "Something went wrong");
        throw new Error(data.message || "Something went wrong");
      }

      
      setEmail("");

    } catch (error) {
      
      setError("something went wrong");

    } finally {
      setLoading(false);
    }
  };

  return (
    <>
    <div className="forgetPass-container">
      <div className="forgetPass-card">
        <h1>SpendWise</h1>
        <h2>Password recovery</h2>
        <p>Enter your email to receive a reset link</p>

        {error && <div className="error-message">{error}</div>}
        {message && <div className="success-message">{message}</div>}

        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <input
              autoFocus
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              type="text"
              placeholder="Enter your email"
              required
              disabled={loading}
            />
            <button type="submit" className="submit-btn" disabled={loading}>
              {loading ? "Sending..." : "Send a reset link"}
            </button>
          </div>
        </form>

        <div className="login-link">
          <a href="/Login">Back to login</a>
        </div>

        <p className="-forgetPass-mark">© 2026 SpendWise</p>

    <div>
      <p className="forgetPass-footer">By using Spendee you agree with SpendWise Terms of Use, Privacy Policy</p>
    </div>
      </div>
    </div>

    </>
  );
}
