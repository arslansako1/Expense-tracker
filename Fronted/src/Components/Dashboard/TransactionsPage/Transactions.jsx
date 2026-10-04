import {useLocation, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState, useTransition } from "react";
import EditTransactionPopup from "./EditTransactionPopup";
import DeleteTransactionPopup from "./DeleteTransactionPopup";
import CreateTransactionPopup from "./CreateTransactionPopup";

export default function Transactions() {
  const navigate = useNavigate();
  const [allTransactions, setAllTransactions] = useState([]);
  const [isEditTransactionPopup, setIsEditTransactionPopup] = useState(false);
  const [isDeleteTransactionPopup, setIsDeleteTransactionPopup] = useState(false);
  const [isCreateTransactionPopup, setIsCreateTransactionPopup] = useState(false);
  const [selectedTransaction, setSelectedTransaction] = useState(null);
  const [selectedAccount, setSelectedAccount] = useState(null);
  const [categories, setCategories] = useState([]);
  const [accounts, setAccounts] = useState([]);
  const [loadingCategory, setLoadingCategory] = useState(false);
  const [loadingAccount, setLoadingAccount] = useState(false);
  const [loading, setLoading] = useState(false);
  const location = useLocation();
  const [error, setError] = useState("");

  const fetchAllTransactions = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getAllTransactions, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all transactions");
        throw new Error("Failed to fetch all transactions");
      }

      const data = await response.json();
      

      const sortedTransactions = data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
      setAllTransactions(sortedTransactions);

    } catch (error) {
      
      setError(`Error fetching all transactions: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

   const fetchAllCategories = async (e) => {
            
            const token = localStorage.getItem("token");
            if (!token){
              
              setLoading(false);
              return;
            }
      
          setLoadingCategory(true);
          setError(null);
      
          
      
          try{
            
            const response = await fetch(API_ENDPOINTS.getAllCategories,{
              headers: {
                Authorization: "Bearer " + token, 
              },
            });
      
            if (!response.ok){
              
              setError("Failed to fetch all categories");
              throw new Error("Failed to fetch all categories");
            }
      
            const data = await response.json();
            
            setCategories(data);
      
          } catch(error){
            
            setError("Error fetching all categories");
      
          } finally{
           setLoadingCategory(false);
          }
        }

         const fetchAllAccounts = async (e) => {
            
            const token = localStorage.getItem("token");
            if (!token){
              
              setLoading(false);
              return;
            }
      
          setLoadingAccount(true);
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
            
            setAccounts(data);
      
          } catch(error){
            
            setError("Error fetching all Accounts");
      
          } finally{
           setLoadingAccount(false);
          }
        }



  const handleCreateTransaction = () => {
    setIsCreateTransactionPopup(true)
  }

  const handleEditTransactionPopup = (transaction, account) => {
    setSelectedAccount(account);
    setSelectedTransaction(transaction);
    setIsEditTransactionPopup(true);

  }

  const handleDeleteTransactionPopup = (transaction, account) => {
    setSelectedAccount(account);
    setSelectedTransaction(transaction);
    setIsDeleteTransactionPopup(true);

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

  useEffect(() => {
    fetchAllTransactions();
    fetchAllCategories();
    fetchAllAccounts();
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
        <h1 className="transcationAccName">All transactions: </h1>

        <button className="viewTransaction-button" onClick={() => handleCreateTransaction()}>Create transaction</button>
        <h3 className="totalTransactions">Total transactions {allTransactions.length}:</h3>

      <div className="viewAcc-transactions">
    
      {allTransactions.length === 0 ? (<p className="notMade">No transactions made yet</p>) : (
        (allTransactions.map((transaction) => (
      <div className="viewTransaction-map" key={transaction.id}>
        <h3>Amount: {transaction.amount}</h3>
        <h3>Account: {transaction.account.name}</h3>
        <h3>type: {transaction.type}</h3>
        <h3>Description: {transaction.description}</h3>
        <h3>Created at: {transaction.createdAt.split("T")[0]}</h3>
        <button className="edit-transaction" onClick={() => handleEditTransactionPopup(transaction, transaction.account)}>Edit transaction</button>
        <button className="delete-transaction" onClick={() => handleDeleteTransactionPopup(transaction, transaction.account)}>Delete transaction</button>


      </div>
      )))
      )}


        {isCreateTransactionPopup && (
          <CreateTransactionPopup accounts={accounts} onClose={() => {setIsCreateTransactionPopup(false); fetchAllTransactions()}}/>
        )}

        {isEditTransactionPopup && (
          <EditTransactionPopup categories={categories} transaction={selectedTransaction} account={selectedAccount} onClose={() => {setIsEditTransactionPopup(false); fetchAllTransactions()}}/>
        )}

        {isDeleteTransactionPopup && (
          <DeleteTransactionPopup transaction={selectedTransaction} account={selectedAccount} onClose={() => {setIsDeleteTransactionPopup(false); fetchAllTransactions()}}/>
        )}
    </div>
    </div>
  )
}
