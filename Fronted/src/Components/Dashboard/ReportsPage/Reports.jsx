import {UNSAFE_SingleFetchRedirectSymbol, useLocation, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import "/src/Css/Dashboard/ReportsPage/Reports.css";


export default function Reports() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [currentMonth, setCurrentMonth] = useState("");
  const [currentNetWorth, setCurrentNetWorth] = useState(0);
  const [endMonth, setEndMonth] = useState("");
  const [endtNetWorth, setEndNetWorth] = useState(0);
  const [income, setIncome] = useState(0);
  const [expense, setExpense] = useState(0);
  const [savings, setSavings] = useState(0);
  const [savingRates, setSavingRates] = useState(0);
  const location = useLocation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const fetchCategoriesSpend = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getCategoriesSpend, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all categories spend");
        throw new Error("Failed to fetch all categories spend");
      }

      const data = await response.json();
      

      setCategories(data);

    } catch (error) {
      
      setError(`Error fetching all transactions:  ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  const fetchBalanceHistory = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getBalanceHistory, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all balance history");
        throw new Error("Failed to fetch all balance history");
      }

      const data = await response.json();
      

      setCurrentMonth(data[0].month);
      setCurrentNetWorth(data[1].networth);

      setEndMonth(data[0].month);
      setEndNetWorth(data[1].networth);

      



    } catch (error) {
      
      setError(`Error fetching balance history: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };


  const fetchIncomeExpense = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getIncomeExpense, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch income and expense spend");
        throw new Error("Failed to fetch income and expense spend");
      }

      const data = await response.json();
      
      
      
      

      setIncome(data.income);
      setExpense(data.expenses);
      setSavings(data.savings);
      setSavingRates(data.savingRates);

      

    } catch (error) {
      
      setError(`Error fetching all income and expense: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };



  function navigateHome(){
    navigate("/MainPage");
  }

  function navigateAccounts(){
    navigate("/Accounts");
  }

  function navigateCategories(){
    navigate("/Categories");
  }

  function navigateBudgets(){
    navigate("/Budgets");
  }

  function navigateTransactions(){
    navigate("/Transactions");
  }

  function navigateTransfers(){
    navigate("/Transfers");
  }

  function navigateAutoPayments(){
    navigate("/AutoPayments");
  }

  function navigateImportBankStatement(){
    navigate("/ImportBankStatement");
  }
  
   function navigateReports(){
    navigate("/Reports");
  }

  function navigateNofications(){
    navigate("/Nofications");
  }

  function navigateProfile(){
    navigate("/Profile");
  }

  function navigateLogout(e){
    e.preventDefault();
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.location.href = "/Login";
  }

  useEffect(() => {
    fetchCategoriesSpend();
    fetchBalanceHistory();
    fetchIncomeExpense();
  }, [])

  return (
    <div>
      <div>
        <header className="mainPage-header">
          <button className="header-btn" onClick={navigateHome}>Home</button>
          <button className="header-btn" onClick={navigateAccounts}>Accounts</button>
          <button className="header-btn" onClick={navigateCategories}>Categories</button>
          <button className="header-btn" onClick={navigateBudgets}>Budgets</button>
          <button className="header-btn" onClick={navigateTransactions}>Transactions</button>
          <button className="header-btn" onClick={navigateTransfers}>Transfers</button>
          <button className="header-btn" onClick={navigateAutoPayments}>Auto payments</button>
          <button className="header-bank" onClick={navigateImportBankStatement}>Import bank statement</button>
          <button className="header-btn" onClick={navigateReports}>Reports</button>
          <button className="header-btn" onClick={navigateNofications}>Nofications</button>
          <button className="header-btn" onClick={navigateProfile}>Profile</button>
          <button className="header-btn" onClick={navigateLogout}>Logout</button>
       </header>            
      </div>

      <div className="reportsPage">

        <h1 className="categories-section">🏷️ Categories spend: </h1>
      {categories.length === 0 ? (<p className="noCategories">No categories yet</p>) : (
        (categories.map((category) => (
      <div className="categoires-map" key={category.id}>
        <h3 className="categoryName">Name: {category.categoryName}</h3>
        <h3 className="categorytotalSpent">TotalSpent: {category.totalSpent}</h3>
        <h3 className="categoryCreatedAt">CreatedAt: {category.createdAt.split("T")[0]}</h3>
    </div>
      )))
      )}

            <h1 className="balance-section">📈 Balance history: </h1>
            <h3 className="balance-currentMonth">Current month: {currentMonth}</h3>
            <h3 className="balance-netWorth">Current netWorth: {currentNetWorth}</h3>
            <h3 className="balance-endMonth">End month: {endMonth}</h3>
            <h3 className="endNetworth">End netWorth: {endtNetWorth}</h3>
  

        <h1 className="incomeExpense-section">💰 Income and expense</h1>
      <h3 className="incomeExpense-income">Income: {income}</h3>
      <h3 className="incomeExpense-expense">Expense: {expense}</h3>
      <h3 className="incomeExpense-savings">Savings: {savings}</h3>
      <h3 className="incomeExpense-savingRates">Saving rates: {savingRates}</h3>

</div>
    </div>
  )
}
