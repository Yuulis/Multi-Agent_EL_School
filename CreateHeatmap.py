import seaborn as sns
import pandas as pd
import numpy as np
import matplotlib as mpl
import matplotlib.pyplot as plt

list = pd.read_csv("./resultData/School_1F_result_2022-11-15-00-25-54.csv").values.tolist()

plt.figure(figsize=(15, 8), dpi=130)
plt.axis("off")
sns.heatmap(list, square=True, cmap='hot', vmin=0, vmax=1000)

# plt.show()
plt.savefig("./result_heatmaps/School_1F_result_2022-11-15-00-25-54.png")