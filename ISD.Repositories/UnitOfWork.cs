using ISD.EntityModels;
using ISD.Repositories.MES;
using ISD.Repositories.MES.IntegrationSAP.MasterData;
using ISD.Repositories.Report;
using ISD.Repositories.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class UnitOfWork
    {
        private EntityDataContext _context;

        private ProfileRepository _profileRepository;
        //private MaterialMasterRepository _materialMasterRepository;
        private TaskRepository _taskRepository;
        private TaskStatusRepository _taskStatusRepository;
        private StoreRepository _storeRepository;
        private WorkFlowRepository _workFlowRepository;
        private CatalogRepository _catalogRepository;
        private SalesEmployeeRepository _repoSaleEmployee;
        private AccountRepository _repoAccount;
        private AppointmentRepository _appointmentRepository;
        private UtilitiesRepository _utilitiesRepository;
        private KanbanTaskRepository _kanbanTaskRepository;
        private CommonDateRepository _commonDateRepository;
        private PersonInChargeRepository _personInChargeRepository;
        private RoleInChargeRepository _roleInChargeRepository;
        private ProfileGroupRepository _profileGroupRepository;
        private CompanyRepository _companyRepository;
        private StockRepository _stockRepository;
        private InventoryRepository _inventoryRepository;
        private DistrictRepository _districtRepository;
        private ProfileCareerRepository _profileCareerRepository;
        private ConstructionRepository _constructionRepository;
        private PivotGridTemplateRepository _pivotGridTemplateRepository;
        private RequestEccEmailConfigRepository _requestEccEmailConfigRepository;
        private SAPReportRepository _SAPReportRepository;
        private WorkShopRepository _WorkShopRepository;
        private ProductionManagementRepository _ProductionManagementRepository;
        private SalesEmployee2Repository _repoSaleEmployee2;
        private SaleOrderHeader100Repository _SaleOrderHeader100Repository;
        private SaleOrderHeader80Repository _SaleOrderHeader80Repository;
        private TimelineRepository _TimelineRepository;
        private ProductionOrderRepository _ProductionOrderRepository;
        private HangtagRepository _HangtagRepository;
        private AssignmentRepository _AssignmentRepository;
        private ConsumableMaterialsDeliveryRepository _ConsumableMaterialsDeliveryRepository;
        private ProductionCompletedStagesReportRepository _ProductionCompletedStagesReportRepository;
        private BC11ReportRepository _BC11ReportRepository;
        private BC12ReportRepository _BC12ReportRepository;
        private BC18ReportRepository _BC18ReportRepository;
        private BC01ReportRepository _BC01ReportRepository;
        private PlantRoutingConfigRepository _PlantRoutingConfigRepository;
        private ContConfigRepository _ContConfigRepository;
        private BC19ReportRepository _BC19ReportRepository;
        private BC02ReportRepository _BC02ReportRepository;
        private BC03ReportRepository _BC03ReportRepository;
        private BC04ReportRepository _BC04ReportRepository;
        private BC05ReportRepository _BC05ReportRepository;
        private BC06ReportRepository _BC06ReportRepository;
        private BC07ReportRepository _BC07ReportRepository;
        private QualityControlRepository _QualityControlRepository;
        private BC15ReportRepository _BC15ReportRepository;
        private BC00ReportRepository _BC00ReportRepository;
        private BC16ReportRepository _BC16ReportRepository;
        private BC20ReportRepository _BC20ReportRepository;
        private BC21ReportRepository _BC21ReportRepository;
        private LookUpProductionStageRepository _lookUpProductStageRepository;
        private WorkOrderRepository _workOrderRepository;
        public UnitOfWork(EntityDataContext entities)
        {
            _context = entities;
        }
        #region MES
        //Integration Material Master
        //public MaterialMasterRepository MaterialMasterRepository
        //{
        //    get
        //    {
        //        if (_materialMasterRepository == null)
        //        {
        //            _materialMasterRepository = new MaterialMasterRepository(_context);
        //        }
        //        return _materialMasterRepository;
        //    }
        //}
        public PlantRoutingConfigRepository PlantRoutingConfigRepository
        {
            get
            {
                if (_PlantRoutingConfigRepository == null)
                {
                    _PlantRoutingConfigRepository = new PlantRoutingConfigRepository(_context);
                }
                return _PlantRoutingConfigRepository;
            }
        }

        public ContConfigRepository ContConfigRepository
        {
            get
            {
                if (_ContConfigRepository == null)
                {
                    _ContConfigRepository = new ContConfigRepository(_context);
                }
                return _ContConfigRepository;
            }
        }

        #endregion

        #region Reports
        public ProductionCompletedStagesReportRepository ProductionCompletedStagesReportRepository
        {
            get
            {
                if (_ProductionCompletedStagesReportRepository == null)
                {
                    _ProductionCompletedStagesReportRepository = new ProductionCompletedStagesReportRepository(_context);
                }
                return _ProductionCompletedStagesReportRepository;
            }
        } 
        
        public BC11ReportRepository BC11ReportRepository
        {
            get
            {
                if (_BC11ReportRepository == null)
                {
                    _BC11ReportRepository = new BC11ReportRepository(_context);
                }
                return _BC11ReportRepository;
            }
        }
        public BC12ReportRepository BC12ReportRepository
        {
            get
            {
                if (_BC12ReportRepository == null)
                {
                    _BC12ReportRepository = new BC12ReportRepository(_context);
                }
                return _BC12ReportRepository;
            }
        }
        public BC18ReportRepository BC18ReportRepository
        {
            get
            {
                if (_BC18ReportRepository == null)
                {
                    _BC18ReportRepository = new BC18ReportRepository(_context);
                }
                return _BC18ReportRepository;
            }
        }
        public BC01ReportRepository BC01ReportRepository
        {
            get
            {
                if (_BC01ReportRepository == null)
                {
                    _BC01ReportRepository = new BC01ReportRepository(_context);
                }
                return _BC01ReportRepository;
            }
        }
        public BC19ReportRepository BC19ReportRepository
        {
            get
            {
                if (_BC19ReportRepository == null)
                {
                    _BC19ReportRepository = new BC19ReportRepository(_context);
                }
                return _BC19ReportRepository;
            }
        }

        public BC02ReportRepository BC02ReportRepository
        {
            get
            {
                if (_BC02ReportRepository == null)
                {
                    _BC02ReportRepository = new BC02ReportRepository(_context);
                }
                return _BC02ReportRepository;
            }
        }

        public BC03ReportRepository BC03ReportRepository
        {
            get
            {
                if (_BC03ReportRepository == null)
                {
                    _BC03ReportRepository = new BC03ReportRepository(_context);
                }
                return _BC03ReportRepository;
            }
        }
        public BC04ReportRepository BC04ReportRepository
        {
            get
            {
                if (_BC04ReportRepository == null)
                {
                    _BC04ReportRepository = new BC04ReportRepository(_context);
                }
                return _BC04ReportRepository;
            }
        }
        public BC05ReportRepository BC05ReportRepository
        {
            get
            {
                if (_BC05ReportRepository == null)
                {
                    _BC05ReportRepository = new BC05ReportRepository(_context);
                }
                return _BC05ReportRepository;
            }
        }

        public BC06ReportRepository BC06ReportRepository
        {
            get
            {
                if (_BC06ReportRepository == null)
                {
                    _BC06ReportRepository = new BC06ReportRepository(_context);
                }
                return _BC06ReportRepository;
            }
        }

        public BC07ReportRepository BC07ReportRepository
        {
            get
            {
                if (_BC07ReportRepository == null)
                {
                    _BC07ReportRepository = new BC07ReportRepository(_context);
                }
                return _BC07ReportRepository;
            }
        }

        public BC15ReportRepository BC15ReportRepository
        {
            get
            {
                if (_BC15ReportRepository == null)
                {
                    _BC15ReportRepository = new BC15ReportRepository(_context);
                }
                return _BC15ReportRepository;
            }
        }
        public BC00ReportRepository BC00ReportRepository
        {
            get
            {
                if (_BC00ReportRepository == null)
                {
                    _BC00ReportRepository = new BC00ReportRepository(_context);
                }
                return _BC00ReportRepository;
            }
        }
        public BC16ReportRepository BC16ReportRepository
        {
            get
            {
                if (_BC16ReportRepository == null)
                {
                    _BC16ReportRepository = new BC16ReportRepository(_context);
                }
                return _BC16ReportRepository;
            }
        }

        public BC20ReportRepository BC20ReportRepository
        {
            get
            {
                if (_BC20ReportRepository == null)
                {
                    _BC20ReportRepository = new BC20ReportRepository(_context);
                }
                return _BC20ReportRepository;
            }
        }

        public BC21ReportRepository BC21ReportRepository
        {
            get
            {
                if (_BC21ReportRepository == null)
                {
                    _BC21ReportRepository = new BC21ReportRepository(_context);
                }
                return _BC21ReportRepository;
            }
        }
        #endregion
        public ConsumableMaterialsDeliveryRepository ConsumableMaterialsDeliveryRepository
        {
            get
            {
                if (_ConsumableMaterialsDeliveryRepository == null)
                {
                    _ConsumableMaterialsDeliveryRepository = new ConsumableMaterialsDeliveryRepository(_context);
                }
                return _ConsumableMaterialsDeliveryRepository;
            }
        }
        public HangtagRepository HangtagRepository
        {
            get
            {
                if (_HangtagRepository == null)
                {
                    _HangtagRepository = new HangtagRepository(_context);
                }
                return _HangtagRepository;
            }
        }   
        public ProductionOrderRepository ProductionOrderRepository
        {
            get
            {
                if (_ProductionOrderRepository == null)
                {
                    _ProductionOrderRepository = new ProductionOrderRepository(_context);
                }
                return _ProductionOrderRepository;
            }
        }   
        
        public AssignmentRepository AssignmentRepository
        {
            get
            {
                if (_AssignmentRepository == null)
                {
                    _AssignmentRepository = new AssignmentRepository(_context);
                }
                return _AssignmentRepository;
            }
        }
        public TimelineRepository TimelineRepository
        {
            get
            {
                if (_TimelineRepository == null)
                {
                    _TimelineRepository = new TimelineRepository(_context);
                }
                return _TimelineRepository;
            }
        }  
        public SaleOrderHeader100Repository SaleOrderHeader100Repository
        {
            get
            {
                if (_SaleOrderHeader100Repository == null)
                {
                    _SaleOrderHeader100Repository = new SaleOrderHeader100Repository(_context);
                }
                return _SaleOrderHeader100Repository;
            }
        } 
        public SaleOrderHeader80Repository SaleOrderHeader80Repository
        {
            get
            {
                if (_SaleOrderHeader80Repository == null)
                {
                    _SaleOrderHeader80Repository = new SaleOrderHeader80Repository(_context);
                }
                return _SaleOrderHeader80Repository;
            }
        } 
        
        public ProfileRepository ProfileRepository
        {
            get
            {
                if (_profileRepository == null)
                {
                    _profileRepository = new ProfileRepository(_context);
                }
                return _profileRepository;
            }
        } 
        public ProductionManagementRepository ProductionManagementRepository
        {
            get
            {
                if (_ProductionManagementRepository == null)
                {
                    _ProductionManagementRepository = new ProductionManagementRepository(_context);
                }
                return _ProductionManagementRepository;
            }
        } 
        
        public WorkShopRepository WorkShopRepository
        {
            get
            {
                if (_WorkShopRepository == null)
                {
                    _WorkShopRepository = new WorkShopRepository(_context);
                }
                return _WorkShopRepository;
            }
        }
       
        public RequestEccEmailConfigRepository RequestEccEmailConfigRepository
        {
            get
            {
                if (_requestEccEmailConfigRepository == null)
                {
                    _requestEccEmailConfigRepository = new RequestEccEmailConfigRepository(_context);
                }
                return _requestEccEmailConfigRepository;
            }
        }
        public PivotGridTemplateRepository PivotGridTemplateRepository
        {
            get
            {
                if (_pivotGridTemplateRepository == null)
                {
                    _pivotGridTemplateRepository = new PivotGridTemplateRepository(_context);
                }
                return _pivotGridTemplateRepository;
            }
        }

        public TaskRepository TaskRepository
        {
            get
            {
                if (_taskRepository == null)
                {
                    _taskRepository = new TaskRepository(_context);
                }
                return _taskRepository;
            }
        }

        public TaskStatusRepository TaskStatusRepository
        {
            get
            {
                if (_taskStatusRepository == null)
                {
                    _taskStatusRepository = new TaskStatusRepository(_context);
                }
                return _taskStatusRepository;
            }
        }

        public StoreRepository StoreRepository
        {
            get
            {
                if (_storeRepository == null)
                {
                    _storeRepository = new StoreRepository(_context);
                }
                return _storeRepository;
            }
        }
        public WorkFlowRepository WorkFlowRepository
        {
            get
            {
                if (_workFlowRepository == null)
                {
                    _workFlowRepository = new WorkFlowRepository(_context);
                }
                return _workFlowRepository;
            }
        }
        public CatalogRepository CatalogRepository
        {
            get
            {
                if (_catalogRepository == null)
                {
                    _catalogRepository = new CatalogRepository(_context);
                }
                return _catalogRepository;
            }
        }
        public SalesEmployeeRepository SalesEmployeeRepository
        {
            get
            {
                if (_repoSaleEmployee == null)
                {
                    _repoSaleEmployee = new SalesEmployeeRepository(_context);
                }
                return _repoSaleEmployee;
            }
        }
        public SalesEmployee2Repository SalesEmployee2Repository
        {
            get
            {
                if (_repoSaleEmployee2 == null)
                {
                    _repoSaleEmployee2 = new SalesEmployee2Repository(_context);
                }
                return _repoSaleEmployee2;
            }
        }
        public AccountRepository AccountRepository
        {
            get
            {
                if (_repoAccount == null)
                {
                    _repoAccount = new AccountRepository(_context);
                }
                return _repoAccount;
            }
        }
        public AppointmentRepository AppointmentRepository
        {
            get
            {
                if (_appointmentRepository == null)
                {
                    _appointmentRepository = new AppointmentRepository(_context);
                }
                return _appointmentRepository;
            }
        }
        public UtilitiesRepository UtilitiesRepository
        {
            get
            {
                if (_utilitiesRepository == null)
                {
                    _utilitiesRepository = new UtilitiesRepository();
                }
                return _utilitiesRepository;
            }
        }
        public KanbanTaskRepository KanbanTaskRepository
        {
            get
            {
                if (_kanbanTaskRepository == null)
                {
                    _kanbanTaskRepository = new KanbanTaskRepository(_context);
                }
                return _kanbanTaskRepository;
            }
        }
        public CommonDateRepository CommonDateRepository
        {
            get
            {
                if (_commonDateRepository == null)
                {
                    _commonDateRepository = new CommonDateRepository(_context);
                }
                return _commonDateRepository;
            }
        }
        public PersonInChargeRepository PersonInChargeRepository
        {
            get
            {
                if (_personInChargeRepository == null)
                {
                    _personInChargeRepository = new PersonInChargeRepository(_context);
                }
                return _personInChargeRepository;
            }
        }
        public RoleInChargeRepository RoleInChargeRepository
        {
            get
            {
                if (_roleInChargeRepository == null)
                {
                    _roleInChargeRepository = new RoleInChargeRepository(_context);
                }
                return _roleInChargeRepository;
            }
        }

        public ProfileGroupRepository ProfileGroupRepository
        {
            get
            {
                if (_profileGroupRepository == null)
                {
                    _profileGroupRepository = new ProfileGroupRepository(_context);
                }
                return _profileGroupRepository;
            }
        }
        public CompanyRepository CompanyRepository
        {
            get
            {
                if (_companyRepository == null)
                {
                    _companyRepository = new CompanyRepository(_context);
                }
                return _companyRepository;
            }
        }
        public StockRepository StockRepository
        {
            get
            {
                if (_stockRepository == null)
                {
                    _stockRepository = new StockRepository(_context);
                }
                return _stockRepository;
            }
        }
        #region Inventory

        private StockReceivingMasterRepository _stockReceivingMasterRepository;
        public StockReceivingMasterRepository StockReceivingMasterRepository
        {
            get
            {
                if (_stockReceivingMasterRepository == null)
                {
                    _stockReceivingMasterRepository = new StockReceivingMasterRepository(_context);
                }
                return _stockReceivingMasterRepository;
            }
        }

        private TransferRepository _transferRepository;
        public TransferRepository TransferRepository
        {
            get
            {
                if (_transferRepository == null)
                {
                    _transferRepository = new TransferRepository(_context);
                }
                return _transferRepository;
            }
        }
        private TransferDetailRepository _transferDetailRepository;
        public TransferDetailRepository TransferDetailRepository
        {
            get
            {
                if (_transferDetailRepository == null)
                {
                    _transferDetailRepository = new TransferDetailRepository(_context);
                }
                return _transferDetailRepository;
            }
        }

        public InventoryRepository InventoryRepository
        {
            get
            {
                if (_inventoryRepository == null)
                {
                    _inventoryRepository = new InventoryRepository(_context);
                }
                return _inventoryRepository;
            }
        }
        #endregion

        private SendSMSRepository _sendSMSRepository;
        public SendSMSRepository SendSMSRepository
        {
            get
            {
                if (_sendSMSRepository == null)
                {
                    _sendSMSRepository = new SendSMSRepository(_context);
                }
                return _sendSMSRepository;
            }
        }
        private RepositoryLibrary _repositoryLibrary;
        public RepositoryLibrary RepositoryLibrary
        {
            get
            {
                if (_repositoryLibrary == null)
                {
                    _repositoryLibrary = new RepositoryLibrary();
                }
                return _repositoryLibrary;
            }
        }

        private ProductRepository _productRepository;
        public ProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }

        private StockRecevingDetailRepository _stockRecevingDetailRepository;
        public StockRecevingDetailRepository StockRecevingDetailRepository
        {
            get
            {
                if (_stockRecevingDetailRepository == null)
                {
                    _stockRecevingDetailRepository = new StockRecevingDetailRepository(_context);
                }
                return _stockRecevingDetailRepository;
            }
        }
        private ProfileLevelRepository _profileLevelRepository;
        public ProfileLevelRepository ProfileLevelRepository
        {
            get
            {
                if (_profileLevelRepository == null)
                {
                    _profileLevelRepository = new ProfileLevelRepository(_context);
                }
                return _profileLevelRepository;
            }
        }

        private CatalogueRepository _catalogueRepository;
        public CatalogueRepository CatalogueRepository
        {
            get
            {
                if (_catalogueRepository == null)
                {
                    _catalogueRepository = new CatalogueRepository(_context);
                }
                return _catalogueRepository;
            }
        }


        private CustomerTasteRepository _customerTasteRepository;
        public CustomerTasteRepository CustomerTasteRepository
        {
            get
            {
                if (_customerTasteRepository == null)
                {
                    _customerTasteRepository = new CustomerTasteRepository(_context);
                }
                return _customerTasteRepository;
            }
        }
        private ProfileContactRepository _profileContactRepository;
        public ProfileContactRepository ProfileContactRepository
        {
            get
            {
                if (_profileContactRepository == null)
                {
                    _profileContactRepository = new ProfileContactRepository(_context);
                }
                return _profileContactRepository;
            }
        }
        private RevenueRepository _revenueRepository;
        public RevenueRepository RevenueRepository
        {
            get
            {
                if (_revenueRepository == null)
                {
                    _revenueRepository = new RevenueRepository(_context);
                }
                return _revenueRepository;
            }
        }
        private ProductWarrantyRepository _productWarrantyRepository;
        public ProductWarrantyRepository ProductWarrantyRepository
        {
            get
            {
                if (_productWarrantyRepository == null)
                {
                    _productWarrantyRepository = new ProductWarrantyRepository(_context);
                }
                return _productWarrantyRepository;
            }
        }
        private WarrantyRepository _warrantyRepository;
        public WarrantyRepository WarrantyRepository
        {
            get
            {
                if (_warrantyRepository == null)
                {
                    _warrantyRepository = new WarrantyRepository(_context);
                }
                return _warrantyRepository;
            }
        }
        private KanbanRepository _kanbanRepository;
        public KanbanRepository KanbanRepository
        {
            get
            {
                if (_kanbanRepository == null)
                {
                    _kanbanRepository = new KanbanRepository(_context);
                }
                return _kanbanRepository;
            }
        }
        private NewsRepository _newsRepository;
        public NewsRepository NewsRepository
        {
            get
            {
                if (_newsRepository == null)
                {
                    _newsRepository = new NewsRepository(_context);
                }
                return _newsRepository;
            }
        }
        private MobileKanbanRepository _mobileKanbanRepository;
        public MobileKanbanRepository MobileKanbanRepository
        {
            get
            {
                if (_mobileKanbanRepository == null)
                {
                    _mobileKanbanRepository = new MobileKanbanRepository(_context);
                }
                return _mobileKanbanRepository;
            }
        }

        private DeliveryRepository _deliveryRepository;
        public DeliveryRepository DeliveryRepository
        {
            get
            {
                if (_deliveryRepository == null)
                {
                    _deliveryRepository = new DeliveryRepository(_context);
                }
                return _deliveryRepository;
            }
        }
        private ConfigUtilities _configUtilities;
        public ConfigUtilities ConfigUtilities
        {
            get
            {
                if (_configUtilities == null)
                {
                    _configUtilities = new ConfigUtilities();
                }
                return _configUtilities;
            }
        }

        private ProvinceRepository _provinceRepository;
        public ProvinceRepository ProvinceRepository
        {
            get
            {
                if (_provinceRepository == null)
                {
                    _provinceRepository = new ProvinceRepository(_context);
                }
                return _provinceRepository;
            }
        }

        public DistrictRepository DistrictRepository
        {
            get
            {
                if (_districtRepository == null)
                {
                    _districtRepository = new DistrictRepository(_context);
                }
                return _districtRepository;
            }
        }

        private AddressBookRepository _addBookRepository;
        public AddressBookRepository AddressBookRepository
        {
            get
            {
                if (_addBookRepository == null)
                {
                    _addBookRepository = new AddressBookRepository(_context);
                }
                return _addBookRepository;
            }
        }

        public ProfileCareerRepository ProfileCareerRepository
        {
            get
            {
                if (_profileCareerRepository == null)
                {
                    _profileCareerRepository = new ProfileCareerRepository(_context);
                }
                return _profileCareerRepository;
            }
        }

        public ConstructionRepository ConstructionRepository
        {
            get
            {
                if (_constructionRepository == null)
                {
                    _constructionRepository = new ConstructionRepository(_context);
                }
                return _constructionRepository;
            }
        }

        public  SAPReportRepository SAPReportRepository
        {
            get
            {
                if (_SAPReportRepository == null)
                {
                    _SAPReportRepository = new SAPReportRepository();
                }
                return _SAPReportRepository;
            }
        }

        public LookUpProductionStageRepository LookUpProductionStageRepository
        {
            get
            {
                if(_lookUpProductStageRepository == null)
                {
                    _lookUpProductStageRepository = new LookUpProductionStageRepository(_context);
                }
                return _lookUpProductStageRepository;
            }
        }

        public WorkOrderRepository WorkOrderRepository
        {
            get
            {
                if(_workOrderRepository == null)
                {
                    _workOrderRepository = new WorkOrderRepository(_context);
                }
                return _workOrderRepository;
            }
        }


        #region QC

        public QualityControlRepository QualityControlRepository
        {
            get
            {
                if (_QualityControlRepository == null)
                {
                    _QualityControlRepository = new QualityControlRepository(_context);
                }
                return _QualityControlRepository;
            }
        }
        #endregion
    }
}
