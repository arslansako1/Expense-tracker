import {useLocation, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import CreateTransferPopup from "./CreateTransferPopup";

export default function Transfers() {
  const navigate = useNavigate();
  const [allTransfers, setAllTransfers] = useState([]);
  const [isCreateTransferPopup, setIsCreateTransferPopup] = useState(false);
  const [selectedTransfer, setSelectedTransfer] = useState(null);
  const [selectedAccount, setSelectedAccount] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const fetchAllTransfers = async (e) => {
    const token = localStorage.getItem("token");
    if (!token) {
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);


    try {
      const response = await fetch(API_ENDPOINTS.getAllTransfers, {
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

  
  const handleCreateTransfer = () => {
    setIsCreateTransferPopup(true)
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
    fetchAllTransfers();
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

      <h1 className="transcationAccName">All transfers: </h1>

        <button className="viewTransaction-button" onClick={() => handleCreateTransfer()}>Create transfer</button>
        <h3 className="totalTransactions">Total transfers {allTransfers.length}:</h3>

      <div className="viewAcc-transactions">
        {allTransfers.length === 0 ? (<p className="notMade">No transfers made</p>) : (
          allTransfers.map((transfer) => (
      <div className="viewTransaction-map" key={transfer.id}>
        <h3>Amount: {transfer.amount}</h3>
        <h3>From account: {transfer.fromAccount.name}</h3>
        <h3>To account: {transfer.toAccount.name}</h3>
        <h3>Description: {transfer.description}</h3>
        <h3>Created at: {transfer.createdAt.split("T")[0]}</h3>
    
   </div>
      ))
      
        )}
 

        {isCreateTransferPopup && (
          <CreateTransferPopup onClose={() => {setIsCreateTransferPopup(false); fetchAllTransfers()}}/>
        )}
    </div>
    </div>
  )
}
