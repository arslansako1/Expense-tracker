import { useLocation, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import CreateTransactionPopup from "./CreateTransactionPopup";
import EditTransactionPopup from "./EditTransactionPopup";
import DeleteTransactionPopup from "./DeleteTransactionPopup";
import "/src/Css/Dashboard/AccountPage/ViewAccTransactions.css";

export default function ViewAccTransactions() {
  const navigate = useNavigate();
  const [allTransactions, setAllTransactions] = useState([]);
  const [isCreateTransactionPopup, setIsCreateTransactionPopup] = useState(false);
  const [isEditTransactionPopup, setIsEditTransactionPopup] = useState(false);
  const [isDeleteTransactionPopup, setIsDeleteTransactionPopup] = useState(false);
  const [selectedAccount, setSelectedAccount] = useState(0);
  const [selectedTransaction, setSelectedTransaction] = useState(null);
  const [allCategories, setAllCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingCategory, setLoadingCategory] = useState(false);
  const location = useLocation();
  const account = location.state?.account;
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
      
      const response = await fetch(API_ENDPOINTS.getAllAccTransactions(account.id), {
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
            
            setAllCategories(data);
      
          } catch(error){
            
            setError("Error fetching all categories");
      
          } finally{
           setLoadingCategory(false);
          }
        }

  const handleCreateTransactionPopup = (account) => {
    setSelectedAccount(account);
    setIsCreateTransactionPopup(true);
    

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
          <button className="header-btn" onClick={navigateNofications}>Nofications</button>
          <button className="header-btn" onClick={navigateProfile}>Profile</button>
          <button className="header-btn" onClick={navigateLogout}>Logout</button>
       </header>            
      </div>
        <h1 className="transcationAccName">{account.name} transactions: </h1>

        <button className="viewTransaction-button" onClick={() => handleCreateTransactionPopup(account)}>Create transaction</button>
        <h3 className="totalTransactions">Total transactions {allTransactions.length}:</h3>

      <div className="viewAcc-transactions">


  {allTransactions.length === 0 ? (<p className="notMade">No transactions made yet</p>) : (
        (allTransactions.map((transaction) => (
      <div className="viewTransaction-map" key={transaction.id}>
        <h3>Amount: {transaction.amount}</h3>
        <h3>type: {transaction.type}</h3>
        <h3>Description: {transaction.description}</h3>
        <h3>Created at: {transaction.createdAt.split("T")[0]}</h3>
        <button className="edit-transaction" onClick={() => handleEditTransactionPopup(transaction, transaction.account)}>Edit transaction</button>
        <button className="delete-transaction" onClick={() => handleDeleteTransactionPopup(transaction, transaction.account)}>Delete transaction</button>


      </div>
      )))
      )}


        {isCreateTransactionPopup && (
          <CreateTransactionPopup accountId={selectedAccount.id} onClose={() => {setIsCreateTransactionPopup(false); fetchAllTransactions()}}/>
        )}

        {isEditTransactionPopup && (
          <EditTransactionPopup transaction={selectedTransaction} categories={allCategories} account={selectedAccount} onClose={() => {setIsEditTransactionPopup(false); fetchAllTransactions()}}/>
        )}

        {isDeleteTransactionPopup && (
          <DeleteTransactionPopup transaction={selectedTransaction} account={selectedAccount} onClose={() => {setIsDeleteTransactionPopup(false); fetchAllTransactions()}}/>
        )}
    </div>
    </div>
  )
}
