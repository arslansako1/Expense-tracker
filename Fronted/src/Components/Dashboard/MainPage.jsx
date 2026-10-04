import { useEffect, useState } from "react";
import { API_ENDPOINTS } from "../../Configration/Urls";
import "/src/Css/Dashboard/MainPage.css";
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";
import Accounts from "./AccountsPage/Accounts";
import "/src/Css/Dashboard/MainPage.css";


export default function MainPage(){
  const navigate = useNavigate();
  const [netWorth, setNetWorth] = useState(0);
  const [allAccounts, setAllAccounts] = useState([]);
  const [allTransactions, setAllTransactions] = useState([]);
  const [allTransfers, setAllTransfers] = useState([]);
  const [allCategories, setAllCategories] = useState([]);
  const [allAutoPayments, setAllAutoPayments] = useState([]);
  const [balanceHistory, setBalanceHistory] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    
    fetchAllAccounts();
    fetchAllTransactions();
    fetchAllTransfers();
    fetchAllCategories();
    fetchAllBudgets();
    fetchAllAutoPayments();

    fetchCategoriesSpend();
    fetchBalanceHistory();
    fetchIncomeExpense();

  }, [])
  


  const formattedDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      month: "long",
      year: "numeric"
    });
  }


  const fetchAllAccounts = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token){
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try{
      
      const response = await fetch(API_ENDPOINTS.getAllAccounts,{
        headers: {
          Authorization: "Bearer " + token, 
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch all accounts");
        throw new Error("Failed to fetch all accounts");
      }

      const data = await response.json();
      
      setAllAccounts(data);

      
      
      

      let totalNetWorth = 0;

      data.map((account ) => {
        const balance = account.balance;
        totalNetWorth += balance;
      });

      setNetWorth(totalNetWorth);
 
     

    } catch(error){
      
      setError("Error fetching all accounts: ", error);

    } finally{
     setLoading(false);
    }
  }

  const fetchAllTransactions = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError("");

    try{
      const response = await fetch(API_ENDPOINTS.getAllTransactions,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch all transactions");
        throw new Error("Failed to fetch all transactions");
      }

      const data = await response.json();
      
      setAllTransactions(data);

    } catch(error){
      
      setError("Error fetching all transactions: ", error);

    } finally{
      setLoading(false);
    }


  }

  const fetchAllTransfers = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError("");

    try{
      const response = await fetch(API_ENDPOINTS.getAllTransfers,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });
      
        if (!response.ok){
          
          setError("Failed to fetch all transfers");
          throw new Error("Failed to fetch all transfers");
        }

        const data = await response.json();
        
        setAllTransfers(data);

    } catch(error){
      setError("Error fetching all transfers: ", error);
      

    } finally{
      setLoading(false);
    }
  }

  const fetchAllCategories = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setError(null)
    setLoading(true);
    
    try{
      const response = await fetch(API_ENDPOINTS.getAllCategories,{
        headers: {
          Authorization: "bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch all categories");
        throw new Error("Failed to fetch all categories");
      }

      const data = await response.json();
      
      setAllCategories(data);

    } catch(error){
      setError("Error fetching all categories: ", error);
      

    } finally{
      setLoading(false);
    }
  }

  const fetchAllBudgets = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }
    
    setError(null);
    setLoading(true);

    try{

      const response = await fetch(API_ENDPOINTS.getAllBudgets,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch all budgets");
        throw new Error("Failed to fetch all budgets");
      }

      const data = await response.json();
      
      setAllBudgets(data);

    } catch(error){
      setError("Error fetching all budgets: ", error);
      

    } finally{
      setLoading(false)
    }
  }

  const fetchAllAutoPayments = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try{
      const response = await fetch(API_ENDPOINTS.getAllRecurringRules,{
        headers:{
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch all auto payments");
        throw new Error("Failed to fetch all auto payments");
      }
      
      const data = await response.json();
      
      setAllAutoPayments(data);


    } catch(error){
     
     setError("Error fetching all auto payments: ", error);

    } finally{
    setLoading(false);
  }
  }

  
  const fetchCategoriesSpend = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try{

      const response = await fetch(API_ENDPOINTS.getCategoriesSpend,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch categories spend");
        throw new Error("Failed to fetch categories spend");
      }

      const data = await response.json();
      
      setBalanceHistory(data);
      

    }
   catch(error){
    
    setError("Error fetching categories spend: ", error);

  } finally{
    setLoading(false);
  }
} 
  
  const fetchBalanceHistory = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try{

      const response = await fetch(API_ENDPOINTS.getBalanceHistory,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch balance history");
        throw new Error("Failed to fetch balance history");
      }

      const data = await response.json();
      
      setBalanceHistory(data);
      

    }
   catch(error){
    
    setError("Error fetching balance history: ", error);

  } finally{
    setLoading(false);
  }
}

  const fetchIncomeExpense = async (e) => {
    const token = localStorage.getItem("token");
    if (!token){
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try{

      const response = await fetch(API_ENDPOINTS.getIncomeExpense,{
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok){
        
        setError("Failed to fetch income expense");
        throw new Error("Failed to fetch income expense");
      }

      const data = await response.json();
      
      setBalanceHistory(data);
      

    }
   catch(error){
    
    setError("Error fetching income expense: ", error);

  } finally{
    setLoading(false);
  }
}

  
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


  return(
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


    <div className="pagge-content">

      <h3 className="nettWorth">Net Worth: {netWorth.toFixed(2)}$</h3>
        
        <Link to="/accounts" className="accountts-link">
         <div className="accountts-square">
        <h3 className="allAccountts">Accounts: </h3>
        {allAccounts.length === 0 ? (
                <p className="noAccountts">No accounts yet</p>
            ) : (allAccounts.slice(0,2).map((account) => (
                <div className="accountts-map" key={account.id}>
                  <h3 className="accountt-name">Name: {account.name}</h3>
                  <h3 className="accountt-balance">{account.balance ? `$${account.balance.toFixed(2)}` : 'N/A'}</h3>
                </div>
            ))
)}
      </div>
      </Link>
     

     <Link to="/transactions" className="transactionns-link"> 
      <div className="transacionns-square">

        <h3 className="allTransactionns">Latest transactions: </h3>

         {allTransactions.length === 0 ? (<p className="noTransactionns">No transactions made yet</p>) : (
        (allTransactions.slice(0,3).map((transaction) => (
      <div className="transactionns-map" key={transaction.id}>
        <h3 className="transactionn-amount">Amount: {transaction.amount}</h3>
        <h3 className="transactionn-description">Description: {transaction.description}</h3>
      </div>
      )))
      )}

      </div>

      </Link>

      <Link to="/Transfers" className="transferrs-link">
      
      <div className="transferrs-square">
         <h3 className="allTransferrs">Latest transfers: </h3>
      {allTransfers.length === 0 ? (<p className="noTransferrs">No transfers made</p>) : allTransfers.slice(0,3).map((transfer) => (
      <div className="transfer-map" key={transfer.id}>
         <h3 className="transferr-amount">Amount: {transfer.amount}</h3>
        <h3 className="transferr-fromAccount">From account: {transfer.fromAccount.name}</h3>
        <h3 className="transferr-toAccount">To account: {transfer.toAccount.name}</h3>

      </div>
      ))}
        
      </div>
      </Link>


      <Link to="/Categories" className="categorries-link">
      <div className="categorries-square">
        <h3 className="allCategorries">Latest categories</h3>
         {allCategories.length === 0 ? (
        <p className="noCategorries">No categories yet</p>
      ) : (
        allCategories.slice(0,3).map((category) => (
          <div className="categorries-map" key={category.id}>
            <input
            className="categorry-input"
              type="text"
              value={`${category.name} ${category.icon}`}
              readOnly
            />
          </div>
        ))
      )}

      </div>
      </Link>


      <Link to="/Budgets" className="budgetts-link">
      <div className="budgetts-square">
        <h3 className="allBudgetts">Latest budgets: </h3>
          {allCategories.length === 0 ? (
          <p className="noCategorries">No Categories yet</p>
        ) : (
          allCategories.map((category) => (
            <div className="categorries-map" key={category.id}>
                 <input
                 className="categorry-input"
                type="text"
                value={`${category.name} ${category.icon}`}
                readOnly
              />
      
              {category.budgets.length === 0 ? (
                <p className="noBudgetts">No budgets yet</p>
               )

               : (category.budgets.slice(0,3).map((budget) => (
                <div className="budgetts-map" key={budget.id}>
                  <h3 className="budgett-createdAt">Created at: {formattedDate(budget.createdAt)}</h3>
                  <h3 className="budgett-monthlyLimit">Monthly limit: {budget.monthlyLimit}</h3>
                </div>
              )))}
       
            </div>
          ))
        )}
        
        
      </div>
      </Link>


      <Link to="/AutoPayments" className="autoPaymentts-link">
      <div className="autoPaymentts-square">
        <h3 className="allAutoPaymentts">Latest auto payemnts: </h3>
          {allAutoPayments.length === 0 ? (
                <p className="noAutoPaymentts">No auto payments yet</p>
            ) : ( allAutoPayments.slice(0,3).map((autoPayment) => (
                <div className="autoPaymentts-map" key={autoPayment.id}>
                    <h3 className="autoPaymentt-amount">Amount: {autoPayment.amount}</h3>
                    <h3 className="autoPaymentt-frequency">Frequency: {autoPayment.frequency}</h3>
                </div>
            ))
)}
      </div>
      </Link>

    </div>
  </div>

    )
}
