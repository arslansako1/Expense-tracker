import { useNavigate } from "react-router-dom";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import { useEffect, useState } from "react";
import CreateBudgetPopup from "./CreateBudgetPopup";
import EditBudgetPopup from "./EditBudgetPopup";
import DeleteBudgetPopup from "./DeleteBudgetPopup";
import "/src/Css/Dashboard/BudgetPage/Budgets.css";

export default function Budgets() {
  const navigate = useNavigate();
  const [Budgets, setBudgets] = useState([]);
  const [categories, setCategories] = useState([]);
  const [isCreateBudgetOpen, setIsCreateBudgetOpen] = useState(false);
  const [isEditBudgetOpen, setIsEditBudgetOpen] = useState(false);
  const [isDeleteBudgetOpen, setIsDeleteBudgetOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [selectedBudget, setSelectedBudget] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const formattedDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      month: "long",
      year: "numeric"
    });
  }

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
      

      const sortedCategories = data.sort(
        (a, b) => new Date(a.createdAt) - new Date(b.createdAt),
      );

      setCategories(sortedCategories);

    } catch (error) {
      
      setError(`Error fetching all categories: ${error}`);

    } finally {
      setLoading(false);
    }
  };

  const fetchAllBudgets = async (e) => {
    const token = localStorage.getItem("token");
    if (!token) {
      
      setError("User must be logged in");
      setLoading(false);
      return;
    }

    setError("");
    setLoading(true);

    try {
      const response = await fetch(API_ENDPOINTS.getAllBudgets, {
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        
        setError("Failed to fetch all budgets");
        throw new Error("Failed to fetch all budgets");
        return;
      }

      const data = await response.json();
      
      
      const sortedBudgets = data.sort(
        (a, b) => new Date(a.createdAt) - new Date(b.createdAt),
      );
      setBudgets(sortedBudgets);

    } catch (error) {
      
      setError(`Error fetching all budgets: ${error.message}`);

    } finally {
      setLoading(false);
    }
  }

    useEffect(() => {
      fetchAllCategories();
      fetchAllBudgets();
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

     function navigateReports(){
    navigate("/Reports");
  }

    function navigateImportBankStatement() {
      navigate("/ImportBankStatement");
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

    const handleCreateBudgetPopup = (category) => {
      setSelectedCategory(category);
      setIsCreateBudgetOpen(true);
    };

    const handleEditBudgetPopup = (budget, category) => {
      setSelectedCategory(category);
      setSelectedBudget(budget);
      setIsEditBudgetOpen(true);
    };

    const handleDeleteBudgetPopup = (budget) => {
      setSelectedBudget(budget);
      setIsDeleteBudgetOpen(true);
    };

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

        <h2 className="budgets-title">Budgets:</h2>
        
      <div className="viewAcc-transactions">
        {categories.length === 0 ? (
          <p className="notMade">No Categories yet</p>
        ) : (
          categories.map((category) => (
            <div className="categories-map" key={category.id}>
              <input
                type="text"
                value={`${category.name} ${category.icon}`}
                readOnly
              />
              {category.budgets.length === 0 ? (
                 <div className="categories-budgets">
                   <p className="no-budgets">No budgets yet</p>
                 </div>)

               : (category.budgets.map((budget) => (
                <div key={budget.id}>
                  <p>2026 {budget.month} {budget.monthlyLimit}$</p>


              <button className="edit-budget" onClick={() => handleEditBudgetPopup(budget, category)}>
                Edit budget
              </button>

              <button className="delete-budget" onClick={() => handleDeleteBudgetPopup(budget)}>
                Delete budget
              </button>
                </div>
              )))}

              <button className="create-budget" onClick={() => handleCreateBudgetPopup(category)}>
                Create budget
              </button>

 
       
            </div>
          ))
        )}

        {isCreateBudgetOpen && (
          <CreateBudgetPopup
            category={selectedCategory}
            onClose={() => {
              setIsCreateBudgetOpen(false);
              fetchAllCategories();
              fetchAllBudgets();
            }}
          />
        )}

        {isEditBudgetOpen && (
          <EditBudgetPopup
            category={selectedCategory}
            budget={selectedBudget}
            onClose={() => {
              setIsEditBudgetOpen(false);
              fetchAllCategories();
              fetchAllBudgets();
            }}
          />
        )}

        {isDeleteBudgetOpen && (
          <DeleteBudgetPopup
            budget={selectedBudget}
            onClose={() => {
              setIsDeleteBudgetOpen(false);
              fetchAllCategories();
              fetchAllBudgets();
            }}
          />
        )}
      </div>
      </div>
    );
}
