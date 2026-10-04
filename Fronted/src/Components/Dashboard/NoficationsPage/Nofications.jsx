import {useLocation, useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import DeleteNoficationPopup from "./DeleteNoficationPopup";
import DeleteAllNoficationPopup from "./DeleteAllNoficationsPopup";
import ReadAllNoficationPopup from "./ReadAllNoficationsPopup";
import UnreadAllNoficationPopup from "./UnreadAllNotificationsPopup";
import "/src/Css/Dashboard/NoficationsPage/Nofications.css";

export default function Nofications() {
  const navigate = useNavigate();
  const [allNofications, setAllNofications] = useState([]);
  const [isDeleteNoficationPopup, setIsDeleteNoficationPopup] = useState(false);
  const [isDeleteAllNoficationPopup, setIsDeleteAllNoficationPopup] = useState(false);
  const [isReadAllNofications, setIsReadAllNofications] = useState(false);
  const [isUnreadAllNofications, setIsUnreadAllNofications] = useState(false);
  const [selectedNofication, setSelectedNofication] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const fetchAllNofications = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);


    

    try {
      
      const response = await fetch(API_ENDPOINTS.getAllNofications, {
        headers: {
          "Authorization": "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all nofications");
        throw new Error("Failed to fetch all nofications");
      }

      const data = await response.json();
      

      const sortedNofications = data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
      setAllNofications(sortedNofications);

    } catch (error) {
      
      setError(`Error fetching all nofications: ${error.message}`);

    } finally {
      setLoading(false);
    }
  };

  const handleMarkNotification = async (nofication) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    const mark = !nofication.isRead


    

    try {
      const response = await fetch(API_ENDPOINTS.markNofication(nofication.id), {
        method: "PUT",
        headers: {
          "Authorization": "Bearer " + token,
          "Content-Type": "application/json"
        },
        body: JSON.stringify({Mark: mark})
      });

      if (!response.ok) {
        
        setError("Failed to mark as read nofications");
        throw new Error("Failed to mark as read nofications");
      }

      

      fetchAllNofications();
     

    } catch (error) {
      
      setError(`Error marking as read nofication: ${error.message}`);

    } finally {
      setLoading(false);
    }
  };


  const handleDeleteNoficationPopup = (nofication) => {
    setSelectedNofication(nofication);
    setIsDeleteNoficationPopup(true);

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
    fetchAllNofications();
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

      <div className="Notifications-center">

        <h1 className="allNofications">🔔 All notifications: </h1>

        <button className="deleteAll" onClick={() => setIsDeleteAllNoficationPopup(true)}>🗑️ Delete all notifications</button>
        <button className="markAll" onClick={() => setIsReadAllNofications(true)}>✅ Mark all as read</button>
        <button className="unmarkAll" onClick={() => setIsUnreadAllNofications(true)}>📩 Mark all as unread</button>
      {allNofications.length === 0 ? (<p className="noNofitications">No notifications yet</p>) : (
        allNofications.map((nofication) => (
      <div className={`nofications-div ${nofication.isRead ? 'read' : 'unread'}`} 
       key={nofication.id}>
        <p className="nofications-message">{nofication.message}</p>
        <button className="delete-nofications" onClick={() => handleDeleteNoficationPopup(nofication)}>Delete</button>
        <button className="mark-nofications" onClick={() => handleMarkNotification(nofication)}>{nofication.isRead ? "Mark as unread" : "Mark as read"}</button>
      </div>
      ))
      )}

        {isDeleteNoficationPopup && (
          <DeleteNoficationPopup nofication={selectedNofication} onClose={() => {setIsDeleteNoficationPopup(false); fetchAllNofications()}}/>
        )}

        {isDeleteAllNoficationPopup && (
          <DeleteAllNoficationPopup  onClose={() => {setIsDeleteAllNoficationPopup(false); fetchAllNofications()}}/>
        )}

        {isReadAllNofications && (
          <ReadAllNoficationPopup  onClose={() => {setIsReadAllNofications(false); fetchAllNofications()}}/>
        )}

        {isUnreadAllNofications && (
          <UnreadAllNoficationPopup  onClose={() => {setIsUnreadAllNofications(false); fetchAllNofications()}}/>
        )}
    </div>
    </div>
  )
}
