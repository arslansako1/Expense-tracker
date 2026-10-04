import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import Popup from "./CreateAccPopup";
import CreateAccPopup from "./CreateAccPopup";
import CreateTransactionPopup from "./CreateTransactionPopup";
import EditAccPopup from "./EditAccPopup";
import AccDeletePopup from "./AccDeletePopup";
import CreateTransferPopup from "./CreateTransferPopup";
import ViewAccTransfers from "./ViewAccTransfers";
import "/src/Css/Dashboard/AccountPage/Accounts.css";


export default function Accounts(){
    const navigate = useNavigate();
    const [accounts, setAccounts] = useState([]);
    const [selectedAccount, setSelectedAccount] = useState(null);
    const [isCreateAccountPopup, setIsCreateAccountPopup] = useState(false);
    const [isCreateTransactionPopup, setIsCreateTransactionPopup] = useState(false);
    const [isCreateTransferPopup, setIsCreateTransferPopup] = useState(false);
    const [isEditAccPopup, setIsEditAccPopup] = useState(false);
    const [isDeleteAccPopup, setIsDeleteAccPopup] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);


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

      const sortedAccounts = data.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
      setAccounts(sortedAccounts);


    } catch(error){
      setError(`Error fetching all accounts: ${error.message}`);

    } finally{
     setLoading(false);
    }
  }

  useEffect(() => {
    fetchAllAccounts();
  }, [])

   
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

  const handleCreateTransactionPopup = (account) => {
    setSelectedAccount(account);
    setIsCreateTransactionPopup(true);
  }

  const handleCreateTransferPopup = (account) => {
    setSelectedAccount(account);
    setIsCreateTransferPopup(true);
  }

  const handleEditAccPopup = (account) => {
    setSelectedAccount(account);
    setIsEditAccPopup(true);
  }

  const handleDeleteAcc = (account) => {
    setSelectedAccount(account);
    setIsDeleteAccPopup(true);
  }

  const navigateViewAccTransactions = (account) => {
    navigate("/ViewAccTransactions", {state: {account}});
  }

  const navigateViewAccTransfers = (account) => {
    navigate("/ViewAccTransfers", {state: {account}});
  }


    return(
        <div className="account-page">
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

       <h2 className="accountPage-text">Manage all your financial accounts in one place</h2>

       <div className="create-account">
            <button onClick={() => setIsCreateAccountPopup(true)}>Create Account</button>
       </div>
  
            {accounts.length === 0 ? (
                <p className="notMade">No accounts yet</p>
            ) : ( accounts.map((account) => (
                <div className="account" key={account.id}>
                  <div className="account-info">
                    <h2 className="account-name">{account.name}</h2>
                    <h3 className="account-type">Type: {account.type}</h3>
                    <h3 className="account-balance">Balance: {account.balance}$</h3>
                    <h3 className="account-currency">Currency: {account.currency}</h3>
                    <h3 className="account-createdAt">Created At: {account.createdAt.split("T")[0]}</h3>
                  </div>
                    
                    <div className="account-buttons">
                      <button className="edit-account" onClick={() => handleEditAccPopup(account)}>Edit account</button>
                      <button className="create-transaction" onClick={() => handleCreateTransactionPopup(account)}>Create transaction</button>
                      <button className="create-transfer" onClick={() => handleCreateTransferPopup(account)}>Create transfer</button>
                      <button className="view-transactions" onClick={() => navigateViewAccTransactions(account)}>View transactions</button>
                      <button className="view-transfers" onClick={() => navigateViewAccTransfers(account)}>View transfers</button>
                      <button className="delete-account" onClick={() => handleDeleteAcc(account)}>Delete account</button>
                    </div>
                </div>
            ))
)}

           
        {isCreateAccountPopup && (
            <CreateAccPopup onClose={() => {setIsCreateAccountPopup(false); fetchAllAccounts();}}/>
        )}

           
        {isCreateTransactionPopup && (
            <CreateTransactionPopup accountId={selectedAccount.id} onClose={() => {setIsCreateTransactionPopup(false); fetchAllAccounts()}}/>
        )}

        {isEditAccPopup && (
            <EditAccPopup account={selectedAccount} onClose={() => {setIsEditAccPopup(false); fetchAllAccounts();}}/>
        )}

        {isDeleteAccPopup && (
          <AccDeletePopup account={selectedAccount} onClose={() => {setIsDeleteAccPopup(false); fetchAllAccounts();}}/>
        )}

        {isCreateTransferPopup && (
          <CreateTransferPopup account={selectedAccount} onClose={() => {setIsCreateTransferPopup(false); fetchAllAccounts();}}/>
        )}

        </div>
    )
}