import { useNavigate, useLocation } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import CreateTransferPopup from "./CreateTransferPopup";
import { useEffect, useState } from "react";

export default function ViewAccTransfers() {
  const navigate = useNavigate();
  const [allTransfers, setAllTransfers] = useState([]);
  const [isCreateTransferPopup, setIsCreateTransferPopup] = useState(false);
  const [selectedAccount, setSelectedAccount] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const location = useLocation();
  const account = location.state?.account;


  const fetchAllTransfers = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
        setError("You must be logged in");
      setLoading(false);
      return;
    }

    if (!account || !account.id){
        
        setError("Account not found");
        setLoading(false)
        return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getAllAccTransfers(account.id), {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all transfers");
        throw new Error("Failed to fetch all transfers");
      }

      const data = await response.json();
      
      const sortedTransfers = data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
      setAllTransfers(sortedTransfers);

    } catch (error) {
      
      setError(`Error fetching all transfers: ${error.message}`);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTransferPopup = (account) => {
    setSelectedAccount(account);
    setIsCreateTransferPopup(true);
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
    if (account?.id){
      fetchAllTransfers();
    }

  }, [account?.id])

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

        <h1 className="transcationAccName">{account.name} transfers: </h1>

        <button className="viewTransaction-button" onClick={() => handleCreateTransferPopup(account)}>Create transfer</button>
        <h3 className="totalTransactions">Total transfers {allTransfers.length}:</h3>

      <div className="viewAcc-transactions">
        {allTransfers.length === 0 ? (<p className="notMade">No transfers made</p>) : (
          allTransfers.map((transfer) => (
      <div className="viewTransaction-map" key={transfer.id}>
        <h3>Amount: {transfer.amount}</h3>
        <h3>toAccount: {transfer.toAccount.name}</h3>
        <h3>Description: {transfer.description}</h3>
        <h3>Created at: {transfer.createdAt.split("T")[0]}</h3>
    
   </div>
      ))
      
        )}
       {isCreateTransferPopup && (
            <CreateTransferPopup account={selectedAccount} onClose={() => {setIsCreateTransferPopup(false); fetchAllTransfers();}}/>
        )}
    </div>
    </div>
  )
}
