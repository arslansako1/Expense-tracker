import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import CreateAutoPaymentPopup from "./CreateAutoPaymentPopup";
import DeleteAutoPaymentPopup from "./DeleteAutoPaymentPopup"; 
import EditAutoPaymentPopup from "./EditAutoPaymentPopup";
import "/src/Css/Dashboard/AutoPaymentsPage/AutoPayments.css";


export default function AutoPayments(){
    const navigate = useNavigate();
    const [allAutoPayments, setAllAutoPayments] = useState([]);
    const [selectedAutoPayment, setSelectedAutoPayment] = useState(null);
    const [isCreateAutoPaymentPopup, setIsCreateAutoPaymentPopup] = useState(false);
    const [isEditAutoPaymentPopup, setIsEditAutoPaymentPopup] = useState(false);
    const [isDeleteAutoPaymentPopup, setIsDeleteAutoPaymentPopup] = useState(false);
    const [allCategories, setAllCategories] = useState([]);
    const [accounts, setAccounts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);


    const fetchAllAutoPayments = async (e) => {
      
      const token = localStorage.getItem("token");
      if (!token){
        
        setLoading(false);
        return;
      }

    setError(null);
    setLoading(true);

    
    

    try{
      
      const response = await fetch(API_ENDPOINTS.getAllRecurringRules,{
        headers: {
          Authorization: "Bearer " + token, 
        },
      });

    setLoading(true);

      if (!response.ok){
        
        setError("Failed to fetch all accounts auto payments");
        throw new Error("Failed to fetch all accounts auto payments");
      }

      const data = await response.json();

      

      const sortedAutoPayments = data.sort((a, b) => new Date(a.nextRunDate) - new Date(b.nextRunDate))

      setAllAutoPayments(sortedAutoPayments);


    } catch(error){
      
      setError("Error fetching all auto payments");

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
      
          setLoading(true);
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
           setLoading(false);
          }
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
            
            setAccounts(data);
      
          } catch(error){
            
            setError("Error fetching all accounts", error);
      
          } finally{
           setLoading(false);
          }
        }


  useEffect(() => {
    fetchAllAutoPayments();
    fetchAllCategories();
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

  const handleCreateAutoPaymentPopup = () => {
    setIsCreateAutoPaymentPopup(true);
  }

  const handleEditAutoPaymentPopup = (autoPayment) => {
    setSelectedAutoPayment(autoPayment);
    setIsEditAutoPaymentPopup(true);
  }

  const handleDeleteAutoPaymentPopup = (autoPayment) => {
    setSelectedAutoPayment(autoPayment);
    setIsDeleteAutoPaymentPopup(true);
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

       <div className="autoPayments-page">

       </div>

        <h2 className="autoPayments-title">Auto Payments:</h2>

  
            <button className="create-autoPayment" onClick={() => handleCreateAutoPaymentPopup()}>+ Create auto payment</button>
            {allAutoPayments.length === 0 ? (
                <p className="notMade">No auto payments yet</p>
            ) : ( allAutoPayments.map((autoPayment) => (
                <div className="autoPayments-map" key={autoPayment.id}>
                    <h3 className="autoPayment-account">Account: {autoPayment.account.name}</h3>
                    <h3 className="autoPayment-amount">Amount: {autoPayment.amount}</h3>
                    <h3 className="autoPayment-category">Category: {autoPayment.category.name}</h3>
                    <h3 className="autoPayment-frequency">Frequency: {autoPayment.frequency}</h3>
                    <h3 className="autoPayment-nextRunDate">Next run date: {autoPayment.nextRunDate.split("T")[0]}</h3>

                    <div className="card-actions">
                      <button className="edit-autoPayment" onClick={() => handleEditAutoPaymentPopup(autoPayment)}>Edit auto payment</button>
                      <button className="delete-autoPayment" onClick={() => handleDeleteAutoPaymentPopup(autoPayment)}>Delete auto payment</button>
                    </div>
                  
                </div>
            ))
)}

           
        {isCreateAutoPaymentPopup && (
            <CreateAutoPaymentPopup onClose={() => {setIsCreateAutoPaymentPopup(false); fetchAllAutoPayments();}}/>
        )}        

        {isEditAutoPaymentPopup && (
            <EditAutoPaymentPopup accounts={accounts} categories={allCategories} autoPayment={selectedAutoPayment} onClose={() => {setIsEditAutoPaymentPopup(false); fetchAllAutoPayments();}}/>
        )}

        {isDeleteAutoPaymentPopup && (
          <DeleteAutoPaymentPopup autoPayment={selectedAutoPayment} onClose={() => {setIsDeleteAutoPaymentPopup(false); fetchAllAutoPayments();}}/>
        )}

        </div>
    )
}