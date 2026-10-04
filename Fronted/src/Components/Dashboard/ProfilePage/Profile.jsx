import { useEffect, useState } from "react";
import { Link, useNavigate } from 'react-router-dom'
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/ProfilePage/Profile.css";


export default function Profile(){
    const navigate = useNavigate();
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [originalFirstName, setOriginalFirstName] = useState("");
    const [originalLastName, setOriginalLastName] = useState("");
    const [originalEmail, setOriginalEmail] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [saved, setSaved] = useState(false);
    const [noChanges, setNoChanges] = useState(false);

    
    
    useEffect(() => {
        const userData = JSON.parse(localStorage.getItem("user"));

        if (userData){
        setFirstName(userData.firstName || "");
        setLastName(userData.lastName || "");
        setEmail(userData.email || "");

        setOriginalFirstName(userData.firstName || "");
        setOriginalLastName(userData.lastName || "");
        setOriginalEmail(userData.email || "");

       } else{
        setError("No userData found");
       }  
     }, []);

    


    const handleSaveBtn = async (e) => {
        if (e) e.preventDefault();

            if (!firstName.trim()) {
              setError("First name cannot be empty");
              setNoChanges(true);
              return;
            }

            if (!lastName.trim()) {
              setError("Last name cannot be empty");
              setNoChanges(true);
              return;
            }

            if (!email.trim()) {
             setError("Email cannot be empty");
             setNoChanges(true);
             return;
            }

        

        const firstNameChange = firstName !== originalFirstName;
        const lastNameChange = lastName !== originalLastName;
        const emailChange = email !== originalEmail;

        if (!firstNameChange && !lastNameChange && !emailChange){
            
            setError("No changes to save");
            setNoChanges(true);
            return;
        }

        setError("");
        setLoading(true);

        const token = localStorage.getItem("token");
        if (!token){
            setError("You must be logged in");
            setLoading(false);
            return;
        }



        try{
            const userData = {
                FirstName: firstName,
                LastName: lastName,
                Email: email
            };
            
            
          const response = await fetch(API_ENDPOINTS.updateMe,{
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },

                body: JSON.stringify(userData)
        });

            const text = await response.text();

            if (!response.ok){
                
                setError("Response failed");
                throw new Error(text || "Response failed");
            };

            const data = JSON.parse(text);
            const user = JSON.parse(localStorage.getItem("user"))
            user.firstName = data.firstName;
            user.lastName = data.lastName;
            user.email = data.email;
            localStorage.setItem("user", JSON.stringify(user));
            
            setSaved(true);

        }catch(error){
        
        setError(`Error updating profil: ${error.message}`);
    }  finally{
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
        <div className="dashboard-container">
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


            <div className="profile-section">
                <form onSubmit={handleSaveBtn}>
                    <h3>Your profile:</h3>

                     <label className="firstName">FirstName:</label>
                    <input autoFocus type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)}/>

                     <label className="lastName">LastName:</label>
                    <input autoFocus type="text" value={lastName} onChange={(e) => setLastName(e.target.value)}/>

                     <label className="email">Email:</label>
                    <input autoFocus type="text"  value={email} onChange={(e) => setEmail(e.target.value)}/>

                    <button className="save-btn" type="submit">{"Save"}</button>
                    {saved && <span>Profile updated successfully</span>}
                    {noChanges && <span>No changes detected</span>}

                </form>
                <div className="profile-singup">
                 <Link to="/MainPage">Back to main page</Link>
                </div>

            </div>
        </div>
    )


    }

