import { useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import CreateCategoryPopup from "./CreateCategoryPopup";
import EditCategoryPopup from "./EditCategoryPopup";
import DeleteCategoryPopup from "./DeleteCategoryPopup";
import ViewCategoryTransactionsPopup from "./ViewCategoryTransactionsPopup";

export default function Categories() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [isCreateCategoryOpen, setIsCreateCategoryOpen] = useState(false);
  const [isEditCategoryOpen, setIsEditCategoryOpen] = useState(false);
  const [isDeleteCategoryOpen, setIsDeleteCategoryOpen] = useState(false);
  const [isViewCategoryTransactions, setIsViewCategoryTransactions] = useState(false);
  const [allTransactions, setAllTransactions] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchAllCategories = async (e) => {
    
    const token = localStorage.getItem("token");
    if (!token) {
      
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    

    try {
      
      const response = await fetch(API_ENDPOINTS.getAllCategories, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all categories");
        throw new Error("Failed to fetch all categories");
      }

      const data = await response.json();
      

      const sortedCategories = data.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
      setCategories(sortedCategories);

    } catch (error) {
      
      setError(`Error fetching all categories: ${error}`);
    } finally {
      setLoading(false);
    }
  };


    const fetchAllCategoryTransactions = async () => {

        const token = localStorage.getItem("token");
        if (!token){
            setLoading(false);
            setError("User must be logged in");
            return;
        }

        setLoading(true);
        setError(null);



        try{
            var response = await fetch(API_ENDPOINTS.getAllTransactions,{
                headers: {
                    "Authorization": "Bearer " + token
                },
            });

            if (!response.ok){
                
                setError("Failed to get all the transactions");
                throw new Error("Failed get all the transactions");
            }

            const data = await response.json();
            const sortedTransactions = data.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
            setAllTransactions(sortedTransactions);

 
            

            


        } catch(error){
            setError("Error geting all the transactions");
            

        } finally{
            setLoading(false);
        }

    }

  useEffect(() => {
    fetchAllCategories();
    fetchAllCategoryTransactions();
  }, []);

  function navigateHome() {
    navigate("/MainPage");
  }

  function navigateAccounts() {
    navigate("/Accounts");
  }

  function navigateCategories() {
    navigate("/Categories");
  }

  function navigateBudgets() {
    navigate("/Budgets");
  }

  function navigateTransactions() {
    navigate("/Transactions");
  }

  function navigateTransfers() {
    navigate("/Transfers");
  }

  function navigateAutoPayments() {
    navigate("/AutoPayments");
  }

  function navigateImportBankStatement() {
    navigate("/ImportBankStatement");
  }

   function navigateReports(){
    navigate("/Reports");
  }

  function navigateNofications() {
    navigate("/Nofications");
  }

  function navigateProfile() {
    navigate("/Profile");
  }

  function navigateLogout(e) {
    e.preventDefault();
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.location.href = "/Login";
  }

  const handleEditCategoryPopup = (category) => {
    setSelectedCategory(category);
    setIsEditCategoryOpen(true);  

  };

  const handleDeleteCategoryPopup = (category) => {
    setSelectedCategory(category);
    setIsDeleteCategoryOpen(true);

  };
  
  const handleViewCategoryTransactionsPopup = (category) => {
    setSelectedCategory(category);
    setIsViewCategoryTransactions(true);

  }

  return (
    <div>
      <header className="mainPage-header">
        <button className="header-btn" onClick={navigateHome}>
          Home
        </button>
        <button className="header-btn" onClick={navigateAccounts}>
          Accounts
        </button>
        <button className="header-btn" onClick={navigateCategories}>
          Categories
        </button>
        <button className="header-btn" onClick={navigateBudgets}>
          Budgets
        </button>
        <button className="header-btn" onClick={navigateTransactions}>
          Transactions
        </button>
        <button className="header-btn" onClick={navigateTransfers}>
          Transfers
        </button>
        <button className="header-btn" onClick={navigateAutoPayments}>
          Auto payments
        </button>
        <button className="header-bank" onClick={navigateImportBankStatement}>
          Import bank statement
        </button>
        <button className="header-btn" onClick={navigateReports}>Reports</button>
        <button className="header-btn" onClick={navigateNofications}>
          Nofications
        </button>
        <button className="header-btn" onClick={navigateProfile}>
          Profile
        </button>
        <button className="header-btn" onClick={navigateLogout}>
          Logout
        </button>
      </header>

        <h2 className="budgets-title">Categories: </h2>

      <button className="create-budget" onClick={() => setIsCreateCategoryOpen(true)}>
        Create category
      </button>

      <div className="viewAcc-transactions">

      {categories.length === 0 ? (
        <p className="notMade">No categories yet</p>
      ) : (
        categories.map((category) => (
          <div className="categories-map" key={category.id}>
            <input
              type="text"
              value={`${category.name} ${category.icon}`}
              readOnly
            />
            <button className="view-transactions" onClick={() => handleViewCategoryTransactionsPopup(category)}>
              View transactions
            </button>
            <button className="edit-budget" onClick={() => handleEditCategoryPopup(category)}>
              Edit category
            </button>
            <button className="delete-budget" onClick={() => handleDeleteCategoryPopup(category)}>
              Delete category
            </button>
          </div>
        ))
      )}

      {isCreateCategoryOpen && (
        <CreateCategoryPopup onClose={() => {setIsCreateCategoryOpen(false); fetchAllCategories();}} />
      )}

      {isEditCategoryOpen && (
        <EditCategoryPopup category={selectedCategory} onClose={() => {setIsEditCategoryOpen(false); fetchAllCategories()}} />
      )}

      {isDeleteCategoryOpen && (
        <DeleteCategoryPopup category={selectedCategory} onClose={() => {setIsDeleteCategoryOpen(false); fetchAllCategories();}} />
      )}

      {isViewCategoryTransactions && (
        <ViewCategoryTransactionsPopup allTransactions={allTransactions} category={selectedCategory} onClose={() => {setIsViewCategoryTransactions(false); fetchAllCategories();}} />
      )}

    </div>
    </div>


  );
}
