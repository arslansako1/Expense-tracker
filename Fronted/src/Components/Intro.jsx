import { Navigate, useNavigate } from "react-router-dom";
import "../Css/Intro.css";

export default function Intro() {
  const navigate = useNavigate();

  function handleLoginBtn() {
    navigate("/Login");
  }

  function handleSignupBtn() {
    navigate("/Signup");
  }

  return (
    <div className="Intro-container">
      <div className="-intro-header">
        <h1 className="hero-title">
          Welcome to the best website <br />
          <span className="highlight-text">to manage your money</span>
        </h1>
      </div>

      <div className="intro-btns">
        <button className="login-btn-intro" onClick={handleLoginBtn}>
          Login
        </button>
        <button className="signup-btn-intro" onClick={handleSignupBtn}>
          Signup
        </button>
      </div>

      <div className="feature-grid">
        <div className="feature-card1">
          <h3>Track every penny</h3>
          <p>
            Easily log your income and expenses.
            <br />
            Categorize every transaction and see
            <br />
            your spending habits at a glance
          </p>
        </div>

        <div className="feature-card2">
          <h3>Set goals and stay on track</h3>
          <p>
            Define monthly budgets for different
            <br /> categories like Groceries, Dining, or Utilities.
            <br /> Get alerts before you overspend
          </p>
        </div>

        <div className="feature-card3">
          <h3>Understand your finance</h3>
          <p>
            View interactive charts of your spending by category,
            <br />
            track your net worth over time,
            <br />
            and compare your income to your expenses
          </p>
        </div>

        <div className="feature-card4">
          <h3>Bank grade security</h3>
          <p>
            Your data is encrypted and secured.
            <br /> Just like a real bank, every transaction
            <br /> is logged and audited for your peace of mind
          </p>
        </div>
      </div>
      <p className="intro-mark">© 2026 SpendWise</p>
    </div>
  );
}
